using Application.Common.Interfaces.Repositories;
using Domain.Entities.Iam;
using Domain.Entities.Profiles;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private static readonly Guid DefaultCampusId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public static string ComputeStudentAccountCode(Guid userId)
    {
        return "VE-" + userId.ToString("N")[..6].ToUpperInvariant();
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

    public async Task<(IReadOnlyList<UserWithProfileInfo> Items, int TotalCount, int ActiveCount, int InactiveCount)> GetUsersPagedAsync(
        string? role = null,
        Guid? campusId = null,
        string? search = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var baseCountQuery = _dbSet.AsNoTracking();
        var activeCount = await baseCountQuery.CountAsync(u => u.IsActive, cancellationToken);
        var inactiveCount = await baseCountQuery.CountAsync(u => !u.IsActive, cancellationToken);

        var query = _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking();

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

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

        return (items, totalCount, activeCount, inactiveCount);
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

    public async Task AddAdministratorProfileAsync(Guid adminId, Guid campusId, CancellationToken cancellationToken = default)
    {
        await AddAdministratorProfileAsync(adminId, cancellationToken);
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

    public async Task<StudentParentLinkOverviewDto?> GetStudentParentLinkOverviewAsync(
        Guid studentUserId,
        bool regeneratePin = false,
        CancellationToken cancellationToken = default)
    {
        var studentUser = await _dbSet.FirstOrDefaultAsync(u => u.UserId == studentUserId, cancellationToken);
        if (studentUser == null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var activePin = await _context.OtpVerifications
            .Where(o => o.Email == studentUser.Email && o.Type == "PARENT_LINK_CODE" && !o.IsUsed && o.ExpiresAt > now.AddMinutes(1))
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (regeneratePin || activePin == null)
        {
            var oldPins = await _context.OtpVerifications
                .Where(o => o.Email == studentUser.Email && o.Type == "PARENT_LINK_CODE" && !o.IsUsed)
                .ToListAsync(cancellationToken);

            foreach (var old in oldPins)
            {
                old.IsUsed = true;
            }

            var newPinCode = Random.Shared.Next(100000, 999999).ToString();
            activePin = new OtpVerification
            {
                Email = studentUser.Email,
                OtpCode = newPinCode,
                Type = "PARENT_LINK_CODE",
                ExpiresAt = now.AddMinutes(30),
                IsUsed = false,
                CreatedAt = now
            };
            await _context.OtpVerifications.AddAsync(activePin, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Lấy danh sách phụ huynh đã liên kết
        var relations = await _context.ParentStudentRelations
            .Where(r => r.StudentId == studentUserId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        var parentIds = relations.Select(r => r.ParentId).Distinct().ToList();
        var parentUsers = parentIds.Count > 0
            ? await _dbSet.Where(u => parentIds.Contains(u.UserId)).ToDictionaryAsync(u => u.UserId, cancellationToken)
            : new Dictionary<Guid, User>();

        var linkedParents = relations.Select(r =>
        {
            parentUsers.TryGetValue(r.ParentId, out var pu);
            return new LinkedParentItemDto(
                r.ParentId,
                pu?.FullName ?? "Phụ huynh",
                pu?.Email ?? string.Empty,
                pu?.Phone ?? string.Empty,
                r.CreatedAt
            );
        }).ToList();

        // Lấy danh sách yêu cầu liên kết từ phụ huynh đang chờ học sinh xác nhận
        var rawRequests = await _context.OtpVerifications
            .Where(o => o.Email == studentUser.Email && o.Type.StartsWith("PARENT_REQ:") && !o.IsUsed && o.ExpiresAt > now)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        var reqParentIds = new List<Guid>();
        foreach (var req in rawRequests)
        {
            var pidStr = req.Type["PARENT_REQ:".Length..];
            if (Guid.TryParse(pidStr, out var pid))
            {
                reqParentIds.Add(pid);
            }
        }

        var reqParentUsers = reqParentIds.Count > 0
            ? await _dbSet.Where(u => reqParentIds.Contains(u.UserId)).ToDictionaryAsync(u => u.UserId, cancellationToken)
            : new Dictionary<Guid, User>();

        var pendingRequests = new List<PendingParentLinkRequestItemDto>();
        foreach (var req in rawRequests)
        {
            var pidStr = req.Type["PARENT_REQ:".Length..];
            if (Guid.TryParse(pidStr, out var pid) && reqParentUsers.TryGetValue(pid, out var pUser))
            {
                pendingRequests.Add(new PendingParentLinkRequestItemDto(
                    req.Id,
                    pid,
                    pUser.FullName,
                    pUser.Email,
                    pUser.Phone,
                    req.OtpCode,
                    req.CreatedAt,
                    req.ExpiresAt
                ));
            }
        }

        return new StudentParentLinkOverviewDto(
            studentUser.UserId,
            studentUser.FullName,
            studentUser.Email,
            ComputeStudentAccountCode(studentUser.UserId),
            activePin.OtpCode,
            activePin.ExpiresAt,
            linkedParents,
            pendingRequests
        );
    }

    public async Task<ParentLinkResultDto> ConnectParentToStudentByCodeAsync(
        Guid parentUserId,
        string codeOrLinkInput,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(codeOrLinkInput))
        {
            return new ParentLinkResultDto(false, "Vui lòng nhập Mã liên kết tài khoản học sinh hoặc dán Đường liên kết mời.");
        }

        var raw = codeOrLinkInput.Trim();

        // Nếu người dùng dán nguyên đường liên kết URL (ví dụ: http://localhost:5173/?parentLinkCode=123456)
        if (raw.Contains("parentLinkCode=", StringComparison.OrdinalIgnoreCase))
        {
            var idx = raw.IndexOf("parentLinkCode=", StringComparison.OrdinalIgnoreCase) + "parentLinkCode=".Length;
            var rest = raw[idx..];
            var ampIdx = rest.IndexOf('&');
            raw = ampIdx >= 0 ? rest[..ampIdx] : rest;
            raw = Uri.UnescapeDataString(raw).Trim();
        }
        else if (raw.Contains("studentCode=", StringComparison.OrdinalIgnoreCase))
        {
            var idx = raw.IndexOf("studentCode=", StringComparison.OrdinalIgnoreCase) + "studentCode=".Length;
            var rest = raw[idx..];
            var ampIdx = rest.IndexOf('&');
            raw = ampIdx >= 0 ? rest[..ampIdx] : rest;
            raw = Uri.UnescapeDataString(raw).Trim();
        }

        var normalizedCode = raw.ToUpperInvariant();
        var now = DateTime.UtcNow;
        User? studentUser = null;
        OtpVerification? matchedOtp = null;

        // 1. Kiểm tra nếu là mã PIN 6 số trong bảng OtpVerifications (PARENT_LINK_CODE hoặc PARENT_REQ)
        if (normalizedCode.Length == 6 && normalizedCode.All(char.IsDigit))
        {
            matchedOtp = await _context.OtpVerifications
                .Where(o => o.OtpCode == normalizedCode && !o.IsUsed && o.ExpiresAt > now &&
                            (o.Type == "PARENT_LINK_CODE" || o.Type.StartsWith("PARENT_REQ:")))
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (matchedOtp != null)
            {
                studentUser = await _dbSet.FirstOrDefaultAsync(u => u.Email == matchedOtp.Email, cancellationToken);
            }
        }

        // 2. Kiểm tra nếu là Mã Định Danh Tài Khoản Học Sinh (VE-XXXXXX hoặc 6 ký tự đầu của UserId hoặc GUID đầy đủ)
        if (studentUser == null)
        {
            var cleanHex = normalizedCode.StartsWith("VE-")
                ? normalizedCode[3..].Trim()
                : normalizedCode.Replace("-", "").Trim();

            if (Guid.TryParse(raw, out var exactStudentGuid))
            {
                studentUser = await _dbSet.FirstOrDefaultAsync(u => u.UserId == exactStudentGuid, cancellationToken);
            }
            else if (cleanHex.Length >= 6 && cleanHex.All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F')))
            {
                var prefixLower = cleanHex.ToLowerInvariant();
                var allCandidates = await _dbSet
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .ToListAsync(cancellationToken);

                studentUser = allCandidates.FirstOrDefault(u =>
                    u.UserId.ToString("N").StartsWith(prefixLower, StringComparison.OrdinalIgnoreCase));
            }
        }

        if (studentUser == null)
        {
            return new ParentLinkResultDto(
                false,
                "Mã liên kết không chính xác hoặc đã hết hạn. Vui lòng kiểm tra lại Mã Tài Khoản (VE-XXXXXX) hoặc Mã PIN 6 số trên màn hình của con.");
        }

        if (studentUser.UserId == parentUserId)
        {
            return new ParentLinkResultDto(false, "Không thể tự liên kết với chính tài khoản của bạn.");
        }

        await EnsureParentAndStudentRelationAsync(parentUserId, studentUser, cancellationToken);

        if (matchedOtp != null)
        {
            matchedOtp.IsUsed = true;
            _context.OtpVerifications.Update(matchedOtp);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return new ParentLinkResultDto(
            true,
            $"Liên kết thành công với học sinh {studentUser.FullName} ({studentUser.Email})!",
            studentUser.UserId,
            studentUser.FullName,
            studentUser.Email,
            ComputeStudentAccountCode(studentUser.UserId));
    }

    public async Task<ParentLinkResultDto> CreateParentLinkRequestAsync(
        Guid parentUserId,
        string studentEmailOrPhoneOrCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(studentEmailOrPhoneOrCode))
        {
            return new ParentLinkResultDto(false, "Vui lòng nhập Email, Số điện thoại hoặc Mã tài khoản của học sinh.");
        }

        var input = studentEmailOrPhoneOrCode.Trim();
        var inputLower = input.ToLowerInvariant();

        // Nếu người dùng nhập thẳng Mã Code VE-XXXXXX hoặc mã PIN 6 số vào ô này, hỗ trợ kết nối luôn
        if (input.StartsWith("VE-", StringComparison.OrdinalIgnoreCase) ||
            (input.Length == 6 && input.All(char.IsDigit)))
        {
            return await ConnectParentToStudentByCodeAsync(parentUserId, input, cancellationToken);
        }

        var studentUser = await _dbSet
            .FirstOrDefaultAsync(
                u => u.Email.ToLower() == inputLower || u.Phone == input,
                cancellationToken);

        if (studentUser == null)
        {
            return new ParentLinkResultDto(
                false,
                $"Không tìm thấy tài khoản học sinh nào khớp với '{input}'. Vui lòng kiểm tra lại Email hoặc Số điện thoại của con.");
        }

        if (studentUser.UserId == parentUserId)
        {
            return new ParentLinkResultDto(false, "Không thể gửi yêu cầu liên kết cho chính tài khoản của bạn.");
        }

        var alreadyLinked = await _context.ParentStudentRelations
            .AnyAsync(r => r.ParentId == parentUserId && r.StudentId == studentUser.UserId, cancellationToken);

        if (alreadyLinked)
        {
            return new ParentLinkResultDto(
                true,
                $"Tài khoản học sinh {studentUser.FullName} ({studentUser.Email}) đã được liên kết với bạn.",
                studentUser.UserId,
                studentUser.FullName,
                studentUser.Email,
                ComputeStudentAccountCode(studentUser.UserId));
        }

        var reqType = $"PARENT_REQ:{parentUserId}";
        var existingReqs = await _context.OtpVerifications
            .Where(o => o.Email == studentUser.Email && o.Type == reqType && !o.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var old in existingReqs)
        {
            old.IsUsed = true;
        }

        var verificationCode = Random.Shared.Next(100000, 999999).ToString();
        var reqOtp = new OtpVerification
        {
            Email = studentUser.Email,
            OtpCode = verificationCode,
            Type = reqType,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.OtpVerifications.AddAsync(reqOtp, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ParentLinkResultDto(
            true,
            $"Đã gửi yêu cầu liên kết tới học sinh {studentUser.FullName} ({studentUser.Email}). Học sinh có thể bấm 'Chấp nhận liên kết' ngay trên Cổng Học Viên hoặc đọc mã xác nhận 6 số trên màn hình của con cho bạn!",
            studentUser.UserId,
            studentUser.FullName,
            studentUser.Email,
            ComputeStudentAccountCode(studentUser.UserId));
    }

    public async Task<ParentLinkResultDto> RespondToParentLinkRequestAsync(
        Guid studentUserId,
        Guid requestId,
        bool approve,
        CancellationToken cancellationToken = default)
    {
        var studentUser = await _dbSet.FirstOrDefaultAsync(u => u.UserId == studentUserId, cancellationToken);
        if (studentUser == null)
        {
            return new ParentLinkResultDto(false, "Không tìm thấy tài khoản học sinh.");
        }

        var reqOtp = await _context.OtpVerifications
            .FirstOrDefaultAsync(o => o.Id == requestId && o.Email == studentUser.Email && !o.IsUsed, cancellationToken);

        if (reqOtp == null || !reqOtp.Type.StartsWith("PARENT_REQ:"))
        {
            return new ParentLinkResultDto(false, "Yêu cầu liên kết không tồn tại hoặc đã được xử lý trước đó.");
        }

        var parentIdStr = reqOtp.Type["PARENT_REQ:".Length..];
        if (!Guid.TryParse(parentIdStr, out var parentUserId))
        {
            return new ParentLinkResultDto(false, "Định danh phụ huynh trong yêu cầu không hợp lệ.");
        }

        reqOtp.IsUsed = true;
        _context.OtpVerifications.Update(reqOtp);

        if (!approve)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return new ParentLinkResultDto(true, "Đã từ chối yêu cầu liên kết từ phụ huynh.");
        }

        await EnsureParentAndStudentRelationAsync(parentUserId, studentUser, cancellationToken);

        return new ParentLinkResultDto(
            true,
            "Đã xác nhận liên kết với tài khoản Phụ huynh thành công!",
            studentUser.UserId,
            studentUser.FullName,
            studentUser.Email,
            ComputeStudentAccountCode(studentUser.UserId));
    }

    private async Task EnsureParentAndStudentRelationAsync(
        Guid parentUserId,
        User studentUser,
        CancellationToken cancellationToken)
    {
        var parentProfile = await _context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentUserId, cancellationToken);
        if (parentProfile == null)
        {
            var parentUser = await _dbSet.FirstOrDefaultAsync(u => u.UserId == parentUserId, cancellationToken);
            parentProfile = new Parent
            {
                ParentId = parentUserId,
                PhoneWork = parentUser?.Phone,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Parents.AddAsync(parentProfile, cancellationToken);
        }

        var studentProfile = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentUser.UserId, cancellationToken);
        if (studentProfile == null)
        {
            studentProfile = new Student
            {
                StudentId = studentUser.UserId,
                CampusId = DefaultCampusId,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Students.AddAsync(studentProfile, cancellationToken);
        }

        var existingRel = await _context.ParentStudentRelations
            .FirstOrDefaultAsync(r => r.ParentId == parentUserId && r.StudentId == studentUser.UserId, cancellationToken);

        if (existingRel == null)
        {
            await _context.ParentStudentRelations.AddAsync(new ParentStudentRelation
            {
                ParentId = parentUserId,
                StudentId = studentUser.UserId,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
