using Application.Features.Students.Commands.UpdateStudentProfile;
using Application.Features.Students.Queries.GetStudentProfile;
using Application.Features.Users.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V_Eval_Identity_Service.API.Controllers.Base;

namespace V_Eval_Identity_Service.API.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
public class StudentsController : ApiControllerBase
{
    /// <summary>
    /// UC 04: Lấy thông tin hồ sơ học sinh (Điểm mục tiêu, cơ sở đào tạo, ngày thi...)
    /// </summary>
    [HttpGet("me/profile")]
    [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentProfile()
    {
        var result = await Mediator.Send(new GetStudentProfileQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// UC 04 & UC 10: Thiết lập / Cập nhật hồ sơ học sinh & Cơ sở đào tạo (Bước 1 của Core Flow 1)
    /// </summary>
    [HttpPut("me/profile")]
    [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateStudentProfile([FromBody] UpdateStudentProfileCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
