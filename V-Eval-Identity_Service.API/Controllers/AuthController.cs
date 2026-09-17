using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.VerifyAccount;
using Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;
using V_Eval_Identity_Service.API.Controllers.Base;

namespace V_Eval_Identity_Service.API.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
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
}
