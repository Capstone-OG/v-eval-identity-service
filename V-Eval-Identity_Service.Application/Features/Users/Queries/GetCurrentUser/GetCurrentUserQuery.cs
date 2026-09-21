using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<Result<UserProfileDto>>;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<UserProfileDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetCurrentUserQuery request,
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

        var roles = user.UserRoles
            .Where(ur => ur.Role != null)
            .Select(ur => ur.Role.RoleName)
            .ToList();

        var profile = new UserProfileDto(
            user.UserId,
            user.Email,
            user.FullName,
            user.Phone,
            user.AvatarUrl,
            user.IsActive,
            user.CreatedAt,
            roles
        );

        return Result<UserProfileDto>.Success(profile);
    }
}
