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
            CampusId = student?.CampusId?.ToString() ?? string.Empty
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

        string campusName = string.Empty;
        if (student.CampusId.HasValue)
        {
            var campus = await _context.Campuses.FirstOrDefaultAsync(c => c.CampusId == student.CampusId.Value);
            campusName = campus?.Name ?? string.Empty;
        }

        return new GetStudentSummaryResponse
        {
            StudentId = student.StudentId.ToString(),
            CampusId = student.CampusId?.ToString() ?? string.Empty,
            CampusName = campusName,
            TargetScore = student.TargetScore ?? 0,
            ExamDate = student.ExamDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            StudyHoursDay = student.StudyHoursDay ?? 0,
            Exists = true
        };
    }
}
