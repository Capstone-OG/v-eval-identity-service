namespace Application.Features.Users.DTOs;

public record UserAdminDto(
    Guid UserId,
    string Email,
    string FullName,
    string Phone,
    string? AvatarUrl,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyList<string> Roles,
    Guid? CampusId = null,
    string? CampusName = null,
    string? Specialty = null
);
