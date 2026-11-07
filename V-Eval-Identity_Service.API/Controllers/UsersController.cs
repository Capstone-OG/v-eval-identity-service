using Application.Common.Interfaces.Repositories;
using Application.Features.Users.Commands.ChangePassword;
using Application.Features.Users.Commands.ProvisionUser;
using Application.Features.Users.Commands.ToggleUserStatus;
using Application.Features.Users.Commands.UpdateProfile;
using Application.Features.Users.DTOs;
using Application.Features.Users.ParentLinking;
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
    /// Lấy danh sách toàn bộ tài khoản người dùng trong hệ thống (Hỗ trợ lọc theo Vai trò, Cơ sở, Trạng thái kích hoạt, Tìm kiếm)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedUsersResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? role,
        [FromQuery] Guid? campusId,
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await Mediator.Send(new GetUsersQuery(role, campusId, search, isActive, page, pageSize));
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
    /// Khóa, mở khóa hoặc đặt trạng thái kích hoạt tài khoản
    /// </summary>
    [HttpPatch("{id:guid}/toggle-status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStatus(
        [FromRoute] Guid id,
        [FromQuery] bool? targetStatus = null)
    {
        var result = await Mediator.Send(new ToggleUserStatusCommand(id, targetStatus));
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

    /// <summary>
    /// UC 41a: Lấy thông tin Mã Liên Kết Tài Khoản Học Sinh (VE-XXXXXX), Mã PIN 6 số, QR Link & Danh sách Yêu cầu liên kết từ Phụ huynh
    /// </summary>
    [HttpGet("me/parent-link-overview")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StudentParentLinkOverviewDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentParentLinkOverview(
        [FromQuery] Guid? studentId = null,
        [FromQuery] bool regeneratePin = false)
    {
        var result = await Mediator.Send(new GetStudentParentLinkOverviewQuery(studentId, regeneratePin));
        return HandleResult(result);
    }

    /// <summary>
    /// UC 41b: Phụ huynh liên kết tài khoản học sinh bằng Mã Tài Khoản (VE-XXXXXX), Mã PIN 6 số hoặc Đường Liên Kết / QR
    /// </summary>
    [HttpPost("parent-link/connect")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ParentLinkResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConnectParentByCode([FromBody] ConnectParentByCodeCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 41c: Phụ huynh gửi yêu cầu liên kết 2 chiều theo Email / SĐT của học sinh
    /// </summary>
    [HttpPost("parent-link/request")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ParentLinkResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestParentLink([FromBody] CreateParentLinkRequestCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 41d: Học sinh xác nhận (Chấp nhận / Từ chối) yêu cầu liên kết từ Phụ huynh ngay trên Web
    /// </summary>
    [HttpPost("me/parent-link-requests/{requestId:guid}/respond")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ParentLinkResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RespondParentLinkRequest(
        [FromRoute] Guid requestId,
        [FromBody] RespondParentLinkRequestCommand command)
    {
        var result = await Mediator.Send(command with { RequestId = requestId });
        return HandleResult(result);
    }
}
