using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetUsers;

public record PagedUsersResult(
    IReadOnlyList<UserAdminDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record GetUsersQuery(
    string? Role = null,
    Guid? CampusId = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<PagedUsersResult>>;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<PagedUsersResult>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PagedUsersResult>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _userRepository.GetUsersPagedAsync(
            request.Role,
            request.CampusId,
            request.Search,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(item =>
        {
            var u = item.User;
            var roles = u.UserRoles
                .Where(ur => ur.Role != null)
                .Select(ur => ur.Role.RoleName)
                .ToList();

            return new UserAdminDto(
                u.UserId,
                u.Email,
                u.FullName,
                u.Phone,
                u.AvatarUrl,
                u.IsActive,
                u.CreatedAt,
                roles,
                item.CampusId,
                item.CampusName,
                item.Specialty
            );
        }).ToList();

        return Result<PagedUsersResult>.Success(new PagedUsersResult(dtos, totalCount, request.Page, request.PageSize));
    }
}
