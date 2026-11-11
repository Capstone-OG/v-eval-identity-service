namespace Application.Common.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Gửi email chứa mã OTP 6 số xác thực kích hoạt tài khoản đăng ký mới (UC 01)
    /// </summary>
    Task SendRegistrationOtpEmailAsync(string toEmail, string fullName, string otpCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gửi email chứa mã OTP 6 số đặt lại mật khẩu khi quên mật khẩu (UC 05)
    /// </summary>
    Task SendForgotPasswordOtpEmailAsync(string toEmail, string fullName, string otpCode, CancellationToken cancellationToken = default);
}

