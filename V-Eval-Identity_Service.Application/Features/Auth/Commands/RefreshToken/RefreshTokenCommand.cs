using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Auth.DTOs;
using Domain.Entities.Iam;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
{
    public string RefreshToken { get; init; } = string.Empty;
}

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token không được để trống.");
    }
}

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token không hợp lệ hoặc đã hết hạn."));
        }

        var user = await _userRepository.GetByEmailWithRolesAsync(existingToken.User.Email, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Unauthorized("Auth.UserInvalid", "Người dùng không tồn tại hoặc đã bị vô hiệu hóa."));
        }

        // Revoke token cũ
        existingToken.IsRevoked = true;
        _refreshTokenRepository.Update(existingToken);

        // Sinh token mới
        var roles = user.UserRoles.Where(ur => ur.Role != null).Select(ur => ur.Role.RoleName).ToList();
        var newAccessToken = _jwtProvider.GenerateAccessToken(user, roles);
        var newRefreshTokenString = _jwtProvider.GenerateRefreshToken();

        var newRefreshToken = new Domain.Entities.Iam.RefreshToken
        {
            UserId = user.UserId,
            Token = newRefreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(user.UserId, user.Email, user.FullName, user.Phone, user.AvatarUrl, roles);
        return Result<AuthResponseDto>.Success(new AuthResponseDto(newAccessToken, newRefreshTokenString, userDto));
    }
}
