using Domain.Entities.Iam;

namespace Application.Common.Interfaces.Repositories;

public record UserWithProfileInfo(
    User User,
    Guid? CampusId,
    string? CampusName,
    string? Specialty
);

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<UserWithProfileInfo> Items, int TotalCount)> GetUsersPagedAsync(
        string? role = null,
        Guid? campusId = null,
        string? search = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task AddTeacherProfileAsync(Guid teacherId, Guid campusId, string specialty, CancellationToken cancellationToken = default);
    Task AddAcademicManagerProfileAsync(Guid managerId, Guid campusId, CancellationToken cancellationToken = default);
    Task AddAcademicDirectorProfileAsync(Guid directorId, CancellationToken cancellationToken = default);
    Task AddAdministratorProfileAsync(Guid adminId, CancellationToken cancellationToken = default);
    Task AddParentProfileAsync(Guid parentId, string? phoneWork, CancellationToken cancellationToken = default);
}
