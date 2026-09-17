namespace Application.Features.Auth.DTOs;

public record UserDto(
    Guid UserId,
    string Email,
    string FullName,
    string Phone,
    string? AvatarUrl,
    IReadOnlyList<string> Roles
);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User
);

public record RegisterResponseDto(
    Guid UserId,
    string Email,
    string FullName,
    string Message,
    string? OtpCode = null
);
