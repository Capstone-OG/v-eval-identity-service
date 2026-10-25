using Grpc.Core;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using VEval.Grpc.Identity;

namespace V_Eval_Identity_Service.API.Services;

public class IdentityGrpcService : IdentityGrpc.IdentityGrpcBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<IdentityGrpcService> _logger;

    public IdentityGrpcService(AppDbContext context, ILogger<IdentityGrpcService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task<ValidatePermissionResponse> ValidateUserPermission(
        ValidatePermissionRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
        {
            return new ValidatePermissionResponse { IsValid = false, IsActive = false };
        }

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserId == userGuid);

        if (user == null || !user.IsActive)
        {
            return new ValidatePermissionResponse { IsValid = false, IsActive = false };
        }

        bool hasRole = string.IsNullOrWhiteSpace(request.RequiredRole) ||
                       user.UserRoles.Any(ur => string.Equals(ur.Role.RoleName, request.RequiredRole, StringComparison.OrdinalIgnoreCase));
        
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == userGuid);

        return new ValidatePermissionResponse
        {
            IsValid = hasRole,
            IsActive = user.IsActive,
            RoleName = user.UserRoles.FirstOrDefault()?.Role.RoleName ?? string.Empty,
            CampusId = student != null ? student.CampusId.ToString() : string.Empty
        };
    }

    public override async Task<GetStudentSummaryResponse> GetStudentProfileSummary(
        GetStudentSummaryRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.StudentId, out var studentGuid))
        {
            return new GetStudentSummaryResponse { Exists = false };
        }

        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentGuid);
        if (student == null)
        {
            return new GetStudentSummaryResponse { Exists = false };
        }

        var campus = await _context.Campuses.FirstOrDefaultAsync(c => c.CampusId == student.CampusId);
        string campusName = campus?.Name ?? string.Empty;

        return new GetStudentSummaryResponse
        {
            StudentId = student.StudentId.ToString(),
            CampusId = student.CampusId.ToString(),
            CampusName = campusName,
            TargetScore = student.TargetScore ?? 0,
            ExamDate = student.ExamDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            StudyHoursDay = student.StudyHoursDay ?? 0,
            Exists = true
        };
    }

    public override async Task<GetParentStudentsResponse> GetParentStudents(
        GetParentStudentsRequest request, ServerCallContext context)
    {
        var response = new GetParentStudentsResponse();
        if (!Guid.TryParse(request.ParentId, out var parentGuid))
        {
            return response;
        }

        var relations = await _context.ParentStudentRelations
            .Where(r => r.ParentId == parentGuid)
            .ToListAsync();

        if (!relations.Any())
        {
            return response;
        }

        var studentIds = relations.Select(r => r.StudentId).ToList();

        var students = await _context.Students
            .Where(s => studentIds.Contains(s.StudentId))
            .ToListAsync();

        var users = await _context.Users
            .Where(u => studentIds.Contains(u.UserId))
            .ToListAsync();

        foreach (var rel in relations)
        {
            var user = users.FirstOrDefault(u => u.UserId == rel.StudentId);
            var student = students.FirstOrDefault(s => s.StudentId == rel.StudentId);

            response.Students.Add(new StudentSummaryItem
            {
                StudentId = rel.StudentId.ToString(),
                Name = user?.FullName ?? "Học sinh",
                Email = user?.Email ?? string.Empty,
                CampusId = student != null ? student.CampusId.ToString() : string.Empty
            });
        }

        return response;
    }

    public override async Task<LinkParentResponse> LinkParentStudent(
        LinkParentRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ParentId, out var parentGuid))
        {
            return new LinkParentResponse { Success = false, Message = "Invalid parent ID format" };
        }

        if (string.IsNullOrWhiteSpace(request.StudentEmail))
        {
            return new LinkParentResponse { Success = false, Message = "Student email cannot be empty" };
        }

        var normalizedEmail = request.StudentEmail.Trim().ToLowerInvariant();
        var studentUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (studentUser == null)
        {
            return new LinkParentResponse { Success = false, Message = $"Student with email '{request.StudentEmail}' not found" };
        }

        var parentProfile = await _context.Parents.FirstOrDefaultAsync(p => p.ParentId == parentGuid);
        if (parentProfile == null)
        {
            parentProfile = new Domain.Entities.Profiles.Parent
            {
                ParentId = parentGuid
            };
            _context.Parents.Add(parentProfile);
        }

        var existingRelation = await _context.ParentStudentRelations
            .FirstOrDefaultAsync(r => r.ParentId == parentGuid && r.StudentId == studentUser.UserId);

        if (existingRelation != null)
        {
            return new LinkParentResponse { Success = true, Message = "Student already linked to parent" };
        }

        _context.ParentStudentRelations.Add(new Domain.Entities.Profiles.ParentStudentRelation
        {
            ParentId = parentGuid,
            StudentId = studentUser.UserId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return new LinkParentResponse { Success = true, Message = "Successfully linked parent and student" };
    }
}
