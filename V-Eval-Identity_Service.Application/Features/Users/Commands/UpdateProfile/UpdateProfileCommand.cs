using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Users.DTOs;
using FluentValidation;
using MediatR;

namespace Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest<Result<UserProfileDto>>
{
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public Guid? CampusId { get; init; }
    public int? TargetScore { get; init; }
    public DateOnly? ExamDate { get; init; }
    public double? StudyHoursDay { get; init; }
    public string? SchoolName { get; init; }
}

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên không được để trống.")
            .MaximumLength(150).WithMessage("Họ và tên không được vượt quá 150 ký tự.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .Matches(@"^(0|\+84)[35789][0-9]{8}$").WithMessage("Số điện thoại không hợp lệ (định dạng Việt Nam: 10 chữ số).");

        RuleFor(x => x.TargetScore)
            .InclusiveBetween(0, 990).When(x => x.TargetScore.HasValue)
            .WithMessage("Điểm mục tiêu phải nằm trong khoảng từ 0 đến 990.");

        RuleFor(x => x.StudyHoursDay)
            .InclusiveBetween(0.1, 24.0).When(x => x.StudyHoursDay.HasValue)
            .WithMessage("Thời gian học mỗi ngày phải từ 0.1 đến 24 giờ.");
    }
}

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserProfileDto>> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Result<UserProfileDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "Người dùng chưa được xác thực."));
        }

        var user = await _userRepository.GetByIdWithDetailsAsync(currentUserId.Value, cancellationToken);
        if (user == null)
        {
            return Result<UserProfileDto>.Failure(
                Error.NotFound("User.NotFound", "Không tìm thấy thông tin người dùng."));
        }

        // Cập nhật thông tin cơ bản
        user.FullName = request.FullName.Trim();
        user.Phone = request.Phone.Trim();
        if (request.AvatarUrl != null)
        {
            user.AvatarUrl = request.AvatarUrl;
        }

        _userRepository.Update(user);

        // Cập nhật thông tin student nếu có
        StudentProfileDto? studentProfileDto = null;
        var student = await _studentRepository.GetByIdAsync(user.UserId, cancellationToken);
        if (student != null)
        {
            if (request.CampusId.HasValue) student.CampusId = request.CampusId;
            if (request.TargetScore.HasValue) student.TargetScore = request.TargetScore;
            if (request.ExamDate.HasValue) student.ExamDate = request.ExamDate;
            if (request.StudyHoursDay.HasValue) student.StudyHoursDay = request.StudyHoursDay;
            if (request.SchoolName != null) student.SchoolName = request.SchoolName.Trim();

            _studentRepository.Update(student);

            studentProfileDto = new StudentProfileDto(
                student.StudentId,
                student.CampusId,
                student.TargetScore,
                student.ExamDate,
                student.StudyHoursDay,
                student.SchoolName
            );
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = user.UserRoles
            .Where(ur => ur.Role != null)
            .Select(ur => ur.Role.RoleName)
            .ToList();

        var updatedProfile = new UserProfileDto(
            user.UserId,
            user.Email,
            user.FullName,
            user.Phone,
            user.AvatarUrl,
            user.IsActive,
            user.CreatedAt,
            roles,
            studentProfileDto
        );

        return Result<UserProfileDto>.Success(updatedProfile);
    }
}
