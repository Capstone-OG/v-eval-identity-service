using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Campuses.DTOs;
using MediatR;

namespace Application.Features.Campuses.Queries.GetCampuses;

public record GetCampusesQuery : IRequest<Result<IReadOnlyList<CampusDto>>>;

public class GetCampusesHandler : IRequestHandler<GetCampusesQuery, Result<IReadOnlyList<CampusDto>>>
{
    private readonly ICampusRepository _campusRepository;

    public GetCampusesHandler(ICampusRepository campusRepository)
    {
        _campusRepository = campusRepository;
    }

    public async Task<Result<IReadOnlyList<CampusDto>>> Handle(
        GetCampusesQuery request,
        CancellationToken cancellationToken)
    {
        var campuses = await _campusRepository.GetActiveCampusesAsync(cancellationToken);

        var dtoList = campuses
            .Select(c => new CampusDto(c.CampusId, c.Code, c.Name, c.Address, c.Phone))
            .ToList();

        return Result<IReadOnlyList<CampusDto>>.Success(dtoList);
    }
}
