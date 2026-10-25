using Application.Common.Interfaces.Repositories;
using Domain.Entities.Iam;
using Domain.Entities.Profiles;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email || (email == "admin" && (u.Email == "admin@veval.edu.vn" || u.Email == "admin")), cancellationToken);
    }

    public async Task<User?> GetByIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<(IReadOnlyList<UserWithProfileInfo> Items, int TotalCount)> GetUsersPagedAsync(
        string? role = null,
        Guid? campusId = null,
        string? search = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(u => u.Email.ToLower().Contains(s) || u.FullName.ToLower().Contains(s) || (u.Phone != null && u.Phone.Contains(s)));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleUpper = role.Trim().ToUpper();
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.RoleName == roleUpper));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var p = page > 0 ? page : 1;
        var ps = pageSize > 0 ? pageSize : 50;

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((p - 1) * ps)
            .Take(ps)
            .ToListAsync(cancellationToken);

        var userIds = users.Select(u => u.UserId).ToList();
        var teachers = await _context.Teachers.Where(t => userIds.Contains(t.TeacherId)).ToDictionaryAsync(t => t.TeacherId, cancellationToken);
        var managers = await _context.AcademicManagers.Where(m => userIds.Contains(m.ManagerId)).ToDictionaryAsync(m => m.ManagerId, cancellationToken);
        var students = await _context.Students.Where(s => userIds.Contains(s.StudentId)).ToDictionaryAsync(s => s.StudentId, cancellationToken);
        var campuses = await _context.Campuses.ToDictionaryAsync(c => c.CampusId, cancellationToken);

        var items = users.Select(u =>
        {
            Guid? cid = null;
            string? spec = null;
            if (teachers.TryGetValue(u.UserId, out var t))
            {
                cid = t.CampusId;
                spec = t.Specialty;
            }
            else if (managers.TryGetValue(u.UserId, out var m))
            {
                cid = m.CampusId;
            }
            else if (students.TryGetValue(u.UserId, out var s))
            {
                cid = s.CampusId;
            }

            string? cname = null;
            if (cid.HasValue && campuses.TryGetValue(cid.Value, out var c))
            {
                cname = c.Name;
            }

            return new UserWithProfileInfo(u, cid, cname, spec);
        }).ToList();

        return (items, totalCount);
    }

    public async Task AddTeacherProfileAsync(Guid teacherId, Guid campusId, string specialty, CancellationToken cancellationToken = default)
    {
        await _context.Teachers.AddAsync(new Teacher
        {
            TeacherId = teacherId,
            CampusId = campusId,
            Specialty = specialty,
            Bio = "Giảng viên cơ sở phụ trách luyện thi ĐGNL",
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task AddAcademicManagerProfileAsync(Guid managerId, Guid campusId, CancellationToken cancellationToken = default)
    {
        await _context.AcademicManagers.AddAsync(new AcademicManager
        {
            ManagerId = managerId,
            CampusId = campusId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task AddAcademicDirectorProfileAsync(Guid directorId, CancellationToken cancellationToken = default)
    {
        await _context.AcademicDirectors.AddAsync(new AcademicDirector
        {
            DirectorId = directorId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task AddAdministratorProfileAsync(Guid adminId, CancellationToken cancellationToken = default)
    {
        await _context.Administrators.AddAsync(new Administrator
        {
            AdminId = adminId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task AddParentProfileAsync(Guid parentId, string? phoneWork, CancellationToken cancellationToken = default)
    {
        await _context.Parents.AddAsync(new Parent
        {
            ParentId = parentId,
            PhoneWork = phoneWork,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);
    }
}
