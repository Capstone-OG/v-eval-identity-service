using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Common.Validators;
using Application.Features.Auth.DTOs;
using Domain.Constants;
using Domain.Entities.Iam;
using Domain.Entities.Profiles;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth.Commands.Register;

public record RegisterUserCommand : IRequest<Result<RegisterResponseDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string RoleName { get; init; } = SystemRoles.Student;
    public Guid? CampusId { get; init; }
}

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email không được để trống.")
            .MaximumLength(150).WithMessage("Email không được vượt quá 150 ký tự.")
            .CustomAsync(async (email, context, cancellationToken) =>
            {
                var check = await EmailValidationHelper.ValidateEmailComprehensiveAsync(email, cancellationToken);
                if (!check.IsValid)
                {
                    context.AddFailure("Email", check.Message);
                }
            });

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ hoa.")
            .Matches(@"[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ thường.")
            .Matches(@"[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên không được để trống.")
            .MaximumLength(150).WithMessage("Họ và tên không được vượt quá 150 ký tự.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .Matches(@"^(0|\+84)[35789][0-9]{8}$").WithMessage("Số điện thoại không hợp lệ (định dạng Việt Nam: 10 chữ số).");

        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Vai trò không được để trống.")
            .Must(role => SystemRoles.All.Contains(role.ToUpper()))
            .WithMessage($"Vai trò không hợp lệ. Các vai trò hợp lệ: {string.Join(", ", SystemRoles.All)}");
    }
}

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<RegisterResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IOtpRepository _otpRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IStudentRepository studentRepository,
        IOtpRepository otpRepository,
        IPasswordHasher _passwordHasher,
        IOtpService _otpService,
        IEmailService emailService,
        IUnitOfWork _unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _studentRepository = studentRepository;
        _otpRepository = otpRepository;
        this._passwordHasher = _passwordHasher;
        this._otpService = _otpService;
        _emailService = emailService;
        this._unitOfWork = _unitOfWork;
    }

    public async Task<Result<RegisterResponseDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        // 0. Kiểm tra tính hợp lệ của Email (cú pháp, quy tắc Gmail, chống chuỗi lặp giả mạo, kiểm tra DNS MX)
        var emailCheck = await EmailValidationHelper.ValidateEmailComprehensiveAsync(emailNormalized, cancellationToken);
        if (!emailCheck.IsValid)
        {
            return Result<RegisterResponseDto>.Failure(
                Error.Validation("Auth.InvalidEmail", emailCheck.Message));
        }

        // 1. Kiểm tra email đã tồn tại trong CSDL chưa
        var existingUser = await _userRepository.GetByEmailWithRolesAsync(emailNormalized, cancellationToken);
        if (existingUser != null)
        {
            if (existingUser.IsActive)
            {
                return Result<RegisterResponseDto>.Failure(
                    Error.Conflict("Auth.EmailAlreadyExists", "Email này đã được đăng ký và kích hoạt bởi một tài khoản khác."));
            }

            // Nếu tài khoản trước đó chưa kích hoạt OTP (IsActive == false), cập nhật lại thông tin và cấp mã OTP mới
            existingUser.FullName = request.FullName.Trim();
            existingUser.Phone = request.Phone.Trim();
            existingUser.PasswordHash = _passwordHasher.Hash(request.Password);
            _userRepository.Update(existingUser);

            var resendOtp = _otpService.GenerateOtpCode();
            var resendVerification = new OtpVerification
            {
                Email = emailNormalized,
                OtpCode = resendOtp,
                Type = "REGISTER_VERIFY",
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };
            await _otpRepository.AddAsync(resendVerification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendRegistrationOtpEmailAsync(existingUser.Email, existingUser.FullName, resendOtp, cancellationToken);

            return Result<RegisterResponseDto>.Success(new RegisterResponseDto(
                existingUser.UserId,
                existingUser.Email,
                existingUser.FullName,
                "Đã gửi mã xác thực OTP đến email của bạn. Vui lòng nhập mã OTP để xác minh email và kích hoạt tài khoản.",
                resendOtp
            ));
        }

        var isEmailUnique = await _userRepository.IsEmailUniqueAsync(emailNormalized, cancellationToken);
        if (!isEmailUnique)
        {
            return Result<RegisterResponseDto>.Failure(
                Error.Conflict("Auth.EmailAlreadyExists", "Email này đã được sử dụng bởi một tài khoản khác."));
        }

        // 2. Lấy role tương ứng
        var roleNameUpper = request.RoleName.Trim().ToUpper();
        var role = await _roleRepository.GetByNameAsync(roleNameUpper, cancellationToken);
        if (role == null)
        {
            return Result<RegisterResponseDto>.Failure(
                Error.NotFound("Auth.RoleNotFound", $"Không tìm thấy vai trò '{roleNameUpper}'."));
        }

        // 3. Băm mật khẩu
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 4. Tạo User entity với IsActive = false (chờ xác thực OTP)
        var user = new User
        {
            Email = emailNormalized,
            PasswordHash = passwordHash,
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        // Gán Role
        user.UserRoles.Add(new UserRole
        {
            UserId = user.UserId,
            RoleId = role.RoleId
        });

        await _userRepository.AddAsync(user, cancellationToken);

        // 5. Khởi tạo profile theo Role (Student hoặc Parent)
        if (roleNameUpper == SystemRoles.Student)
        {
            var student = new Student
            {
                StudentId = user.UserId,
                CampusId = (request.CampusId.HasValue && 
                    (request.CampusId.Value == Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6") || 
                     request.CampusId.Value == Guid.Parse("4ba85f64-5717-4562-b3fc-2c963f66afa7")))
                    ? request.CampusId.Value
                    : Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                CreatedAt = DateTime.UtcNow
            };
            await _studentRepository.AddAsync(student, cancellationToken);
        }
        else if (roleNameUpper == SystemRoles.Parent)
        {
            await _userRepository.AddParentProfileAsync(user.UserId, request.Phone, cancellationToken);
        }

        // 6. Sinh mã OTP và lưu vào iam.otp_verifications
        var otpCode = _otpService.GenerateOtpCode();
        var otpVerification = new OtpVerification
        {
            Email = emailNormalized,
            OtpCode = otpCode,
            Type = "REGISTER_VERIFY",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
        await _otpRepository.AddAsync(otpVerification, cancellationToken);

        // 7. Lưu tất cả thay đổi qua UnitOfWork
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. Gửi email chứa mã OTP kích hoạt tài khoản qua Brevo SMTP
        await _emailService.SendRegistrationOtpEmailAsync(user.Email, user.FullName, otpCode, cancellationToken);

        var responseDto = new RegisterResponseDto(
            user.UserId,
            user.Email,
            user.FullName,
            "Đã gửi mã xác thực OTP đến email của bạn. Vui lòng nhập mã OTP để xác minh email và kích hoạt tài khoản.",
            otpCode
        );

        return Result<RegisterResponseDto>.Success(responseDto);
    }
}
