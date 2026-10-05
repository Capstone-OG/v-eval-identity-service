using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Users.DTOs;
using Domain.Constants;
using Domain.Entities.Iam;
using Domain.Entities.Profiles;
using FluentValidation;
using MediatR;

namespace Application.Features.Users.Commands.ProvisionUser;

public record ProvisionUserCommand : IRequest<Result<UserAdminDto>>
{
    public string Email { get; init; } = string.Empty;
    public string? Password { get; init; } = "Password123@";
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string RoleName { get; init; } = SystemRoles.Teacher;
    public Guid? CampusId { get; init; }
    public string? Specialty { get; init; }
}

public class ProvisionUserValidator : AbstractValidator<ProvisionUserCommand>
{
    public ProvisionUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email không được để trống.");
        RuleFor(x => x.FullName).NotEmpty().WithMessage("Họ và tên không được để trống.");
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .Must(r => SystemRoles.All.Contains(r.ToUpper()))
            .WithMessage($"Vai trò không hợp lệ. Hợp lệ: {string.Join(", ", SystemRoles.All)}");
    }
}

public class ProvisionUserHandler : IRequestHandler<ProvisionUserCommand, Result<UserAdminDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICampusRepository _campusRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ProvisionUserHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICampusRepository campusRepository,
        IStudentRepository studentRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _campusRepository = campusRepository;
        _studentRepository = studentRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserAdminDto>> Handle(
        ProvisionUserCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        // 1. Kiểm tra email đã tồn tại hay chưa
        var isUnique = await _userRepository.IsEmailUniqueAsync(emailNormalized, cancellationToken);
        if (!isUnique)
        {
            return Result<UserAdminDto>.Failure(
                Error.Conflict("User.EmailExists", "Email này đã tồn tại trong hệ thống."));
        }

        // 2. Tìm Role tương ứng
        var roleNameUpper = request.RoleName.Trim().ToUpper();
        var role = await _roleRepository.GetByNameAsync(roleNameUpper, cancellationToken);
        if (role == null)
        {
            return Result<UserAdminDto>.Failure(
                Error.NotFound("User.RoleNotFound", $"Không tìm thấy vai trò '{roleNameUpper}'."));
        }

        // 3. Tìm Campus nếu có hoặc lấy campus đầu tiên nếu là Teacher/Manager
        Campus? campus = null;
        if (request.CampusId.HasValue && request.CampusId.Value != Guid.Empty)
        {
            campus = await _campusRepository.GetByIdAsync(request.CampusId.Value, cancellationToken);
        }
        if (campus == null && (roleNameUpper == SystemRoles.Teacher || roleNameUpper == SystemRoles.AcademicManager))
        {
            var activeCampuses = await _campusRepository.GetActiveCampusesAsync(cancellationToken);
            campus = activeCampuses.FirstOrDefault();
        }

        // 4. Hash mật khẩu (Mặc định: Password123@)
        var password = string.IsNullOrWhiteSpace(request.Password) ? "Password123@" : request.Password.Trim();
        var passwordHash = _passwordHasher.Hash(password);

        // 5. Tạo User với IsActive = true (Cấp trực tiếp, kích hoạt ngay không cần OTP)
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = emailNormalized,
            PasswordHash = passwordHash,
            FullName = request.FullName.Trim(),
            Phone = !string.IsNullOrWhiteSpace(request.Phone) ? request.Phone.Trim() : "0900000000",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.UserRoles.Add(new UserRole
        {
            UserId = user.UserId,
            RoleId = role.RoleId
        });

        await _userRepository.AddAsync(user, cancellationToken);

        // 6. Tạo profile tương ứng
        var campusGuid = campus?.CampusId ?? Guid.Empty;
        if (roleNameUpper == SystemRoles.Teacher)
        {
            await _userRepository.AddTeacherProfileAsync(user.UserId, campusGuid, request.Specialty ?? "Toán & Tư duy Logic", cancellationToken);
        }
        else if (roleNameUpper == SystemRoles.AcademicManager)
        {
            await _userRepository.AddAcademicManagerProfileAsync(user.UserId, campusGuid, cancellationToken);
        }
        else if (roleNameUpper == SystemRoles.AcademicDirector)
        {
            await _userRepository.AddAcademicDirectorProfileAsync(user.UserId, cancellationToken);
        }
        else if (roleNameUpper == SystemRoles.Administrator)
        {
            await _userRepository.AddAdministratorProfileAsync(user.UserId, cancellationToken);
        }
        else if (roleNameUpper == SystemRoles.Student)
        {
            await _studentRepository.AddAsync(new Student
            {
                StudentId = user.UserId,
                CampusId = campus?.CampusId,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
        }
        else if (roleNameUpper == SystemRoles.Parent)
        {
            await _userRepository.AddParentProfileAsync(user.UserId, request.Phone, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userAdminDto = new UserAdminDto(
            user.UserId,
            user.Email,
            user.FullName,
            user.Phone,
            user.AvatarUrl,
            user.IsActive,
            user.CreatedAt,
            new[] { roleNameUpper },
            campus?.CampusId,
            campus?.Name,
            request.Specialty
        );

        return Result<UserAdminDto>.Success(userAdminDto);
    }
}
