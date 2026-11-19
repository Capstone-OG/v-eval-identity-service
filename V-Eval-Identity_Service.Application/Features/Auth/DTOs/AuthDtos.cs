namespace Application.Features.Auth.DTOs;

public record UserDto(
    Guid UserId,
    string Email,
    string FullName,
    string Phone,
    string? AvatarUrl,
    IReadOnlyList<string> Roles,
    Guid? CampusId = null,
    string? CampusName = null
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

public record ForgotPasswordResponseDto(
    string Email,
    string Message,
    string? OtpCode = null
);

