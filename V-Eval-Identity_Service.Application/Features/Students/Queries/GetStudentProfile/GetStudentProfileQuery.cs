using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Students.Queries.GetStudentProfile;

public record GetStudentProfileQuery : IRequest<Result<StudentProfileDto>>;

public class GetStudentProfileHandler : IRequestHandler<GetStudentProfileQuery, Result<StudentProfileDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IStudentRepository _studentRepository;

    public GetStudentProfileHandler(
        ICurrentUserService currentUserService,
        IStudentRepository studentRepository)
    {
        _currentUserService = currentUserService;
        _studentRepository = studentRepository;
    }

    public async Task<Result<StudentProfileDto>> Handle(
        GetStudentProfileQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Result<StudentProfileDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "Người dùng chưa được xác thực."));
        }

        var student = await _studentRepository.GetByIdAsync(currentUserId.Value, cancellationToken);
        if (student == null)
        {
            return Result<StudentProfileDto>.Failure(
                Error.NotFound("Student.NotFound", "Không tìm thấy hồ sơ học sinh của người dùng này."));
        }

        var dto = new StudentProfileDto(
            student.StudentId,
            student.CampusId,
            student.TargetScore,
            student.ExamDate,
            student.StudyHoursDay,
            student.SchoolName
        );

        return Result<StudentProfileDto>.Success(dto);
    }
}
