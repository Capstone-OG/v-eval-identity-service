using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
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
}

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Định dạng email không hợp lệ.")
            .MaximumLength(150).WithMessage("Email không được vượt quá 150 ký tự.");

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
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IStudentRepository studentRepository,
        IOtpRepository otpRepository,
        IPasswordHasher _passwordHasher,
        IOtpService _otpService,
        IUnitOfWork _unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _studentRepository = studentRepository;
        _otpRepository = otpRepository;
        this._passwordHasher = _passwordHasher;
        this._otpService = _otpService;
        this._unitOfWork = _unitOfWork;
    }

    public async Task<Result<RegisterResponseDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        // 1. Kiểm tra email đã tồn tại chưa
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

        // 4. Tạo User entity với IsActive = false
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

        // 5. Nếu role là STUDENT, tự khởi tạo bản ghi trong profile.students
        if (roleNameUpper == SystemRoles.Student)
        {
            var student = new Student
            {
                StudentId = user.UserId,
                CreatedAt = DateTime.UtcNow
            };
            await _studentRepository.AddAsync(student, cancellationToken);
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

        var responseDto = new RegisterResponseDto(
            user.UserId,
            user.Email,
            user.FullName,
            "Đăng ký thành công. Vui lòng xác thực tài khoản bằng mã OTP đã được gửi.",
            otpCode // Trả kèm OTP để thuận tiện kiểm thử
        );

        return Result<RegisterResponseDto>.Success(responseDto);
    }
}
