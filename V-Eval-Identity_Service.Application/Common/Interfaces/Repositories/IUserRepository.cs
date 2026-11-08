using Domain.Entities.Iam;

namespace Application.Common.Interfaces.Repositories;

public record UserWithProfileInfo(
    User User,
    Guid? CampusId,
    string? CampusName,
    string? Specialty
);

public record LinkedParentItemDto(
    Guid ParentId,
    string FullName,
    string Email,
    string Phone,
    DateTime LinkedAt
);

public record PendingParentLinkRequestItemDto(
    Guid RequestId,
    Guid ParentId,
    string ParentName,
    string ParentEmail,
    string ParentPhone,
    string VerificationCode,
    DateTime CreatedAt,
    DateTime ExpiresAt
);

public record StudentParentLinkOverviewDto(
    Guid StudentId,
    string StudentName,
    string StudentEmail,
    string StudentAccountCode,
    string ActivePinCode,
    DateTime PinExpiresAt,
    IReadOnlyList<LinkedParentItemDto> LinkedParents,
    IReadOnlyList<PendingParentLinkRequestItemDto> PendingRequests
);

public record ParentLinkResultDto(
    bool Success,
    string Message,
    Guid? StudentId = null,
    string? StudentName = null,
    string? StudentEmail = null,
    string? StudentAccountCode = null
);

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<UserWithProfileInfo> Items, int TotalCount, int ActiveCount, int InactiveCount)> GetUsersPagedAsync(
        string? role = null,
        Guid? campusId = null,
        string? search = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task AddTeacherProfileAsync(Guid teacherId, Guid campusId, string specialty, CancellationToken cancellationToken = default);
    Task AddAcademicManagerProfileAsync(Guid managerId, Guid campusId, CancellationToken cancellationToken = default);
    Task AddAcademicDirectorProfileAsync(Guid directorId, CancellationToken cancellationToken = default);
    Task AddAdministratorProfileAsync(Guid adminId, Guid campusId = default, CancellationToken cancellationToken = default);
    Task AddAdministratorProfileAsync(Guid adminId, CancellationToken cancellationToken = default);
    Task AddParentProfileAsync(Guid parentId, string? phoneWork, CancellationToken cancellationToken = default);

    // Parent - Student Linking Operations (Code / Invite URL / QR / 2-Way Web Approval)
    Task<StudentParentLinkOverviewDto?> GetStudentParentLinkOverviewAsync(
        Guid studentUserId,
        bool regeneratePin = false,
        CancellationToken cancellationToken = default);

    Task<ParentLinkResultDto> ConnectParentToStudentByCodeAsync(
        Guid parentUserId,
        string codeOrLinkInput,
        CancellationToken cancellationToken = default);

    Task<ParentLinkResultDto> CreateParentLinkRequestAsync(
        Guid parentUserId,
        string studentEmailOrPhoneOrCode,
        CancellationToken cancellationToken = default);

    Task<ParentLinkResultDto> RespondToParentLinkRequestAsync(
        Guid studentUserId,
        Guid requestId,
        bool approve,
        CancellationToken cancellationToken = default);
}
