using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Users.DTOs;
using Domain.Entities.Profiles;
using FluentValidation;
using MediatR;

namespace Application.Features.Students.Commands.UpdateStudentProfile;

public record UpdateStudentProfileCommand : IRequest<Result<StudentProfileDto>>
{
    public Guid? CampusId { get; init; }
    public int? TargetScore { get; init; }
    public DateOnly? ExamDate { get; init; }
    public double? StudyHoursDay { get; init; }
    public string? SchoolName { get; init; }
}

public class UpdateStudentProfileValidator : AbstractValidator<UpdateStudentProfileCommand>
{
    public UpdateStudentProfileValidator()
    {
        RuleFor(x => x.TargetScore)
            .InclusiveBetween(0, 1200).When(x => x.TargetScore.HasValue)
            .WithMessage("Điểm mục tiêu phải nằm trong khoảng từ 0 đến 1200 (thang điểm chuẩn V-ACT).");

        RuleFor(x => x.StudyHoursDay)
            .InclusiveBetween(0.1, 24.0).When(x => x.StudyHoursDay.HasValue)
            .WithMessage("Thời gian học mỗi ngày phải từ 0.1 đến 24 giờ.");

        RuleFor(x => x.SchoolName)
            .MaximumLength(255).When(x => x.SchoolName != null)
            .WithMessage("Tên trường học không được vượt quá 255 ký tự.");
    }
}

public class UpdateStudentProfileHandler : IRequestHandler<UpdateStudentProfileCommand, Result<StudentProfileDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentProfileHandler(
        ICurrentUserService currentUserService,
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StudentProfileDto>> Handle(
        UpdateStudentProfileCommand request,
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
            student = new Student
            {
                StudentId = currentUserId.Value,
                CreatedAt = DateTime.UtcNow
            };
            await _studentRepository.AddAsync(student, cancellationToken);
        }

        if (request.CampusId.HasValue) student.CampusId = request.CampusId;
        if (request.TargetScore.HasValue) student.TargetScore = request.TargetScore;
        if (request.ExamDate.HasValue) student.ExamDate = request.ExamDate;
        if (request.StudyHoursDay.HasValue) student.StudyHoursDay = request.StudyHoursDay;
        if (request.SchoolName != null) student.SchoolName = request.SchoolName.Trim();

        _studentRepository.Update(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
