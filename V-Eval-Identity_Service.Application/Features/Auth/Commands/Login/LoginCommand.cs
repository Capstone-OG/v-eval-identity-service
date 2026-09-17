using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Auth.DTOs;
using Domain.Entities.Iam;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth.Commands.Login;

public record LoginCommand : IRequest<Result<AuthResponseDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Định dạng email không hợp lệ.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.");
    }
}

public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LoginHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        // 1. Tìm user kèm vai trò
        var user = await _userRepository.GetByEmailWithRolesAsync(emailNormalized, cancellationToken);
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            // Trả lỗi chung không phân biệt rõ sai email hay password để bảo mật
            return Result<AuthResponseDto>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Thông tin đăng nhập email hoặc mật khẩu không chính xác."));
        }

        // 2. Kiểm tra tài khoản đã kích hoạt chưa
        if (!user.IsActive)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Forbidden("Auth.AccountNotActivated", "Tài khoản chưa được kích hoạt. Vui lòng xác thực mã OTP trước khi đăng nhập."));
        }

        // 3. Trích xuất danh sách role
        var roles = user.UserRoles
            .Where(ur => ur.Role != null)
            .Select(ur => ur.Role.RoleName)
            .ToList();

        // 4. Sinh Access Token (JWT) và Refresh Token
        var accessToken = _jwtProvider.GenerateAccessToken(user, roles);
        var refreshTokenString = _jwtProvider.GenerateRefreshToken();

        // 5. Lưu Refresh Token vào iam.refresh_tokens
        var refreshToken = new Domain.Entities.Iam.RefreshToken
        {
            UserId = user.UserId,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Trả về kết quả
        var userDto = new UserDto(
            user.UserId,
            user.Email,
            user.FullName,
            user.Phone,
            user.AvatarUrl,
            roles
        );

        return Result<AuthResponseDto>.Success(new AuthResponseDto(accessToken, refreshTokenString, userDto));
    }
}
