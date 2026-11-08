using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using MediatR;

namespace Application.Features.Users.ParentLinking;

// 1. Query: Lấy thông tin Mã Liên Kết Tài Khoản Học Sinh (VE-XXXXXX), Mã PIN 6 số, QR Link & Danh sách Yêu cầu liên kết từ Phụ huynh
public sealed record GetStudentParentLinkOverviewQuery(
    Guid? StudentId = null,
    bool RegeneratePin = false
) : IRequest<Result<StudentParentLinkOverviewDto>>;

public sealed class GetStudentParentLinkOverviewHandler
    : IRequestHandler<GetStudentParentLinkOverviewQuery, Result<StudentParentLinkOverviewDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetStudentParentLinkOverviewHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<StudentParentLinkOverviewDto>> Handle(
        GetStudentParentLinkOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var targetId = request.StudentId ?? _currentUserService.UserId;
        if (!targetId.HasValue || targetId.Value == Guid.Empty)
        {
            return Result<StudentParentLinkOverviewDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để xem mã liên kết học sinh."));
        }

        var overview = await _userRepository.GetStudentParentLinkOverviewAsync(
            targetId.Value,
            request.RegeneratePin,
            cancellationToken);

        if (overview == null)
        {
            return Result<StudentParentLinkOverviewDto>.Failure(
                Error.NotFound("User.NotFound", "Không tìm thấy thông tin tài khoản học sinh."));
        }

        return Result<StudentParentLinkOverviewDto>.Success(overview);
    }
}

// 2. Command: Phụ huynh nhập Mã Code Tài Khoản (VE-XXXXXX), Mã PIN 6 số hoặc Dán Đường Liên Kết / Quét QR để kết nối tức thì
public sealed record ConnectParentByCodeCommand(
    string CodeOrLink,
    Guid? ParentId = null
) : IRequest<Result<ParentLinkResultDto>>;

public sealed class ConnectParentByCodeHandler
    : IRequestHandler<ConnectParentByCodeCommand, Result<ParentLinkResultDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public ConnectParentByCodeHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ParentLinkResultDto>> Handle(
        ConnectParentByCodeCommand request,
        CancellationToken cancellationToken)
    {
        var parentId = request.ParentId ?? _currentUserService.UserId;
        if (!parentId.HasValue || parentId.Value == Guid.Empty)
        {
            return Result<ParentLinkResultDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập tài khoản phụ huynh để liên kết."));
        }

        var res = await _userRepository.ConnectParentToStudentByCodeAsync(
            parentId.Value,
            request.CodeOrLink,
            cancellationToken);

        if (!res.Success)
        {
            return Result<ParentLinkResultDto>.Failure(
                Error.Validation("ParentLink.InvalidCode", res.Message));
        }

        return Result<ParentLinkResultDto>.Success(res);
    }
}

// 3. Command: Phụ huynh tìm theo Email / SĐT của con và gửi yêu cầu xác nhận liên kết 2 chiều
public sealed record CreateParentLinkRequestCommand(
    string StudentIdentifier,
    Guid? ParentId = null
) : IRequest<Result<ParentLinkResultDto>>;

public sealed class CreateParentLinkRequestHandler
    : IRequestHandler<CreateParentLinkRequestCommand, Result<ParentLinkResultDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateParentLinkRequestHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ParentLinkResultDto>> Handle(
        CreateParentLinkRequestCommand request,
        CancellationToken cancellationToken)
    {
        var parentId = request.ParentId ?? _currentUserService.UserId;
        if (!parentId.HasValue || parentId.Value == Guid.Empty)
        {
            return Result<ParentLinkResultDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập tài khoản phụ huynh để gửi yêu cầu."));
        }

        var res = await _userRepository.CreateParentLinkRequestAsync(
            parentId.Value,
            request.StudentIdentifier,
            cancellationToken);

        if (!res.Success)
        {
            return Result<ParentLinkResultDto>.Failure(
                Error.Validation("ParentLink.StudentNotFound", res.Message));
        }

        return Result<ParentLinkResultDto>.Success(res);
    }
}

// 4. Command: Học sinh bấm Chấp nhận hoặc Từ chối yêu cầu liên kết từ Phụ huynh ngay trên Web
public sealed record RespondParentLinkRequestCommand(
    Guid RequestId,
    bool Approve,
    Guid? StudentId = null
) : IRequest<Result<ParentLinkResultDto>>;

public sealed class RespondParentLinkRequestHandler
    : IRequestHandler<RespondParentLinkRequestCommand, Result<ParentLinkResultDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RespondParentLinkRequestHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ParentLinkResultDto>> Handle(
        RespondParentLinkRequestCommand request,
        CancellationToken cancellationToken)
    {
        var studentId = request.StudentId ?? _currentUserService.UserId;
        if (!studentId.HasValue || studentId.Value == Guid.Empty)
        {
            return Result<ParentLinkResultDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập tài khoản học sinh để xác nhận."));
        }

        var res = await _userRepository.RespondToParentLinkRequestAsync(
            studentId.Value,
            request.RequestId,
            request.Approve,
            cancellationToken);

        if (!res.Success)
        {
            return Result<ParentLinkResultDto>.Failure(
                Error.Validation("ParentLink.RespondFailed", res.Message));
        }

        return Result<ParentLinkResultDto>.Success(res);
    }
}
