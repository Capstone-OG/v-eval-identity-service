using Application.Features.Users.Commands.ChangePassword;
using Application.Features.Users.Commands.UpdateProfile;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries.GetCurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V_Eval_Identity_Service.API.Controllers.Base;

namespace V_Eval_Identity_Service.API.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
public class UsersController : ApiControllerBase
{
    /// <summary>
    /// UC 04: Lấy thông tin hồ sơ người dùng hiện tại từ JWT
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await Mediator.Send(new GetCurrentUserQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// UC 04 (Extend): Cập nhật thông tin cá nhân & hồ sơ học sinh
    /// </summary>
    [HttpPut("me/profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 04 (Extend): Đổi mật khẩu người dùng
    /// </summary>
    [HttpPost("me/change-password")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
