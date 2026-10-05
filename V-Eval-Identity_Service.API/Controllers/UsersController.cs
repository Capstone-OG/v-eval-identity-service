using Application.Features.Users.Commands.ChangePassword;
using Application.Features.Users.Commands.ProvisionUser;
using Application.Features.Users.Commands.ToggleUserStatus;
using Application.Features.Users.Commands.UpdateProfile;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries.GetCurrentUser;
using Application.Features.Users.Queries.GetUsers;
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
    /// Lấy danh sách toàn bộ tài khoản người dùng trong hệ thống (Hỗ trợ lọc theo Vai trò, Cơ sở, Tìm kiếm)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedUsersResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? role,
        [FromQuery] Guid? campusId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await Mediator.Send(new GetUsersQuery(role, campusId, search, page, pageSize));
        return HandleResult(result);
    }

    /// <summary>
    /// Cấp phát tài khoản nội bộ trực tiếp cho các vai trò (Giảng viên, Quản lý, Giám đốc, Quản trị viên, Phụ huynh, Học sinh)
    /// Tài khoản được kích hoạt ngay lập tức (IsActive = true), không yêu cầu mã OTP email
    /// </summary>
    [HttpPost("provision")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ProvisionUser([FromBody] ProvisionUserCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Khóa hoặc mở khóa trạng thái kích hoạt tài khoản
    /// </summary>
    [HttpPatch("{id:guid}/toggle-status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStatus([FromRoute] Guid id)
    {
        var result = await Mediator.Send(new ToggleUserStatusCommand(id));
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
