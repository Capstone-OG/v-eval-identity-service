using Application.Features.Campuses.DTOs;
using Application.Features.Campuses.Queries.GetCampuses;
using Microsoft.AspNetCore.Mvc;
using V_Eval_Identity_Service.API.Controllers.Base;

namespace V_Eval_Identity_Service.API.Controllers;

[Route("api/v1/[controller]")]
public class CampusesController : ApiControllerBase
{
    /// <summary>
    /// UC 10 & UC 40: Lấy danh sách các cơ sở đào tạo đang hoạt động
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CampusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCampuses()
    {
        var result = await Mediator.Send(new GetCampusesQuery());
        return HandleResult(result);
    }
}
