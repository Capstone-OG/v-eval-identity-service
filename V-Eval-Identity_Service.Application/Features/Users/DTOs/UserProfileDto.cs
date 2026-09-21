namespace Application.Features.Users.DTOs;

public record StudentProfileDto(
    Guid StudentId,
    Guid? CampusId,
    int? TargetScore,
    DateOnly? ExamDate,
    double? StudyHoursDay,
    string? SchoolName
);

public record UserProfileDto(
    Guid UserId,
    string Email,
    string FullName,
    string Phone,
    string? AvatarUrl,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyList<string> Roles
);
