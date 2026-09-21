namespace Application.Features.Campuses.DTOs;

public record CampusDto(
    Guid CampusId,
    string Code,
    string Name,
    string Address,
    string Phone
);
