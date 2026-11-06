using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Logout;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.VerifyResetOtp;
using Application.Features.Auth.Commands.VerifyAccount;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Queries.ValidateEmail;
using Microsoft.AspNetCore.Mvc;
using V_Eval_Identity_Service.API.Controllers.Base;

namespace V_Eval_Identity_Service.API.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    /// <summary>
    /// UC 01a: Kiểm tra tính hợp lệ, máy chủ nhận thư DNS MX và trạng thái tồn tại của Email trước khi đăng ký
    /// </summary>
    [HttpGet("validate-email")]
    [ProducesResponseType(typeof(ValidateEmailResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateEmail([FromQuery] string email)
    {
        var result = await Mediator.Send(new ValidateEmailQuery(email ?? string.Empty));
        return HandleResult(result);
    }

    /// <summary>
    /// UC 01: Đăng ký tài khoản mới (Mặc định vai trò STUDENT)
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 02: Xác thực tài khoản bằng mã OTP 6 số
    /// </summary>
    [HttpPost("verify-account")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyAccount([FromBody] VerifyAccountCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 03: Đăng nhập hệ thống (Cấp JWT Access Token và Refresh Token)
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Làm mới Access Token thông qua Refresh Token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 05: Yêu cầu mã OTP đặt lại mật khẩu khi quên mật khẩu
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ForgotPasswordResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 05b: Xác thực mã OTP đặt lại mật khẩu trước khi qua bước 3
    /// </summary>
    [HttpPost("verify-reset-otp")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyResetOtpCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 06: Xác nhận OTP và đặt lại mật khẩu mới
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// UC 07: Đăng xuất hệ thống (Thu hồi Refresh Token)
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
