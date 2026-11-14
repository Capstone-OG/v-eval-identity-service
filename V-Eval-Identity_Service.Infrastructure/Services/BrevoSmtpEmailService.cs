using Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Services;

public sealed class BrevoSmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BrevoSmtpEmailService> _logger;

    public BrevoSmtpEmailService(
        IConfiguration configuration,
        ILogger<BrevoSmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendRegistrationOtpEmailAsync(
        string toEmail,
        string fullName,
        string otpCode,
        CancellationToken cancellationToken = default)
    {
        var subject = "[V-Eval] Mã xác thực kích hoạt tài khoản của bạn";
        var htmlContent = $$"""
        <!DOCTYPE html>
        <html lang="vi">
        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Xác Thực Tài Khoản - V-Eval</title>
            <style>
                body { margin: 0; padding: 0; background-color: #f1f5f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color: #0f172a; }
                .wrapper { width: 100%; background-color: #f1f5f9; padding: 40px 0; }
                .card { max-width: 560px; margin: 0 auto; background-color: #ffffff; border-radius: 20px; overflow: hidden; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.01); border: 1px solid #e2e8f0; }
                .header { background: linear-gradient(135deg, #1e3a8a 0%, #2563eb 100%); padding: 36px 32px; text-align: center; }
                .brand-badge { display: inline-block; background: rgba(255, 255, 255, 0.15); border: 1px solid rgba(255, 255, 255, 0.3); color: #ffffff; padding: 4px 14px; border-radius: 9999px; font-size: 11px; font-weight: 700; letter-spacing: 1.5px; text-transform: uppercase; margin-bottom: 12px; }
                .brand-title { color: #ffffff; font-size: 24px; font-weight: 800; margin: 0; letter-spacing: -0.5px; }
                .content { padding: 36px 32px; }
                .greeting { font-size: 17px; font-weight: 700; color: #1e293b; margin-bottom: 12px; }
                .text { font-size: 14px; line-height: 1.6; color: #475569; margin: 0 0 24px 0; }
                .otp-box { background: linear-gradient(180deg, #f8fafc 0%, #f1f5f9 100%); border: 2px dashed #93c5fd; border-radius: 16px; padding: 24px; text-align: center; margin: 28px 0; }
                .otp-label { font-size: 11px; font-weight: 800; color: #2563eb; text-transform: uppercase; letter-spacing: 2px; margin-bottom: 8px; }
                .otp-number { font-family: 'Courier New', Courier, monospace; font-size: 38px; font-weight: 900; color: #1e3a8a; letter-spacing: 8px; margin: 0; }
                .notice-box { background-color: #eff6ff; border-left: 4px solid #3b82f6; padding: 14px 18px; border-radius: 0 12px 12px 0; font-size: 13px; color: #1e40af; line-height: 1.5; margin-bottom: 24px; }
                .footer { background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 24px 32px; text-align: center; font-size: 12px; color: #94a3b8; line-height: 1.6; }
            </style>
        </head>
        <body>
            <div class="wrapper">
                <div class="card">
                    <div class="header">
                        <div class="brand-badge">Nền Tảng Đào Tạo Thích Ứng</div>
                        <h1 class="brand-title">V-EVAL LEARNING</h1>
                    </div>
                    <div class="content">
                        <div class="greeting">Kính chào {{fullName}},</div>
                        <p class="text">
                            Cảm ơn bạn đã lựa chọn tin tưởng <strong>V-Eval Platform</strong>. Để hoàn tất đăng ký và kích hoạt tài khoản của bạn, vui lòng nhập mã xác thực OTP (One-Time Password) dưới đây:
                        </p>
                        
                        <div class="otp-box">
                            <div class="otp-label">MÃ XÁC THỰC KÍCH HOẠT</div>
                            <div class="otp-number">{{otpCode}}</div>
                        </div>

                        <div class="notice-box">
                            ⏱️ Mã OTP này có hiệu lực trong vòng <strong>10 phút</strong>.<br>
                            🔒 Tuyệt đối không chia sẻ mã này với bất kỳ ai để bảo vệ an toàn thông tin cá nhân của bạn.
                        </div>

                        <p class="text" style="font-size: 13px; color: #64748b; margin-bottom: 0;">
                            Nếu bạn không thực hiện đăng ký tài khoản tại V-Eval, vui lòng bỏ qua thư này. Tài khoản của bạn sẽ không bị kích hoạt nếu không có mã OTP.
                        </p>
                    </div>
                    <div class="footer">
                        © 2026 V-Eval Adaptive Learning Platform • Bản quyền thuộc ĐHQG-HCM<br>
                        Hỗ trợ kỹ thuật: support@veval.edu.vn | Hotline: 1900 8889
                    </div>
                </div>
            </div>
        </body>
        </html>
        """;

        await SendEmailInternalAsync(toEmail, subject, htmlContent, cancellationToken);
    }

    public async Task SendForgotPasswordOtpEmailAsync(
        string toEmail,
        string fullName,
        string otpCode,
        CancellationToken cancellationToken = default)
    {
        var subject = "[V-Eval] Yêu cầu đặt lại mật khẩu của bạn";
        var htmlContent = $$"""
        <!DOCTYPE html>
        <html lang="vi">
        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Đặt Lại Mật Khẩu - V-Eval</title>
            <style>
                body { margin: 0; padding: 0; background-color: #f1f5f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color: #0f172a; }
                .wrapper { width: 100%; background-color: #f1f5f9; padding: 40px 0; }
                .card { max-width: 560px; margin: 0 auto; background-color: #ffffff; border-radius: 20px; overflow: hidden; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.01); border: 1px solid #e2e8f0; }
                .header { background: linear-gradient(135deg, #4338ca 0%, #6366f1 100%); padding: 36px 32px; text-align: center; }
                .brand-badge { display: inline-block; background: rgba(255, 255, 255, 0.15); border: 1px solid rgba(255, 255, 255, 0.3); color: #ffffff; padding: 4px 14px; border-radius: 9999px; font-size: 11px; font-weight: 700; letter-spacing: 1.5px; text-transform: uppercase; margin-bottom: 12px; }
                .brand-title { color: #ffffff; font-size: 24px; font-weight: 800; margin: 0; letter-spacing: -0.5px; }
                .content { padding: 36px 32px; }
                .greeting { font-size: 17px; font-weight: 700; color: #1e293b; margin-bottom: 12px; }
                .text { font-size: 14px; line-height: 1.6; color: #475569; margin: 0 0 24px 0; }
                .otp-box { background: linear-gradient(180deg, #f8fafc 0%, #f1f5f9 100%); border: 2px dashed #a5b4fc; border-radius: 16px; padding: 24px; text-align: center; margin: 28px 0; }
                .otp-label { font-size: 11px; font-weight: 800; color: #4f46e5; text-transform: uppercase; letter-spacing: 2px; margin-bottom: 8px; }
                .otp-number { font-family: 'Courier New', Courier, monospace; font-size: 38px; font-weight: 900; color: #312e81; letter-spacing: 8px; margin: 0; }
                .warning-box { background-color: #fff1f2; border-left: 4px solid #f43f5e; padding: 14px 18px; border-radius: 0 12px 12px 0; font-size: 13px; color: #be123c; line-height: 1.5; margin-bottom: 24px; }
                .footer { background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 24px 32px; text-align: center; font-size: 12px; color: #94a3b8; line-height: 1.6; }
            </style>
        </head>
        <body>
            <div class="wrapper">
                <div class="card">
                    <div class="header">
                        <div class="brand-badge">Bảo Mật Tài Khoản</div>
                        <h1 class="brand-title">V-EVAL LEARNING</h1>
                    </div>
                    <div class="content">
                        <div class="greeting">Kính chào {{fullName}},</div>
                        <p class="text">
                            Hệ thống vừa nhận được yêu cầu đặt lại mật khẩu cho tài khoản V-Eval liên kết với địa chỉ email này. Vui lòng sử dụng mã OTP dưới đây để tiến hành thiết lập mật khẩu mới:
                        </p>
                        
                        <div class="otp-box">
                            <div class="otp-label">MÃ KHÔI PHỤC MẬT KHẨU</div>
                            <div class="otp-number">{{otpCode}}</div>
                        </div>

                        <div class="warning-box">
                            ⚠️ Mã OTP này có hiệu lực trong vòng <strong>10 phút</strong>.<br>
                            Nếu bạn <strong>không</strong> gửi yêu cầu này, vui lòng đổi mật khẩu ngay hoặc liên hệ ban quản trị để bảo vệ tài khoản.
                        </div>
                    </div>
                    <div class="footer">
                        © 2026 V-Eval Adaptive Learning Platform • Bản quyền thuộc ĐHQG-HCM<br>
                        Hỗ trợ bảo mật: security@veval.edu.vn | Hotline: 1900 8889
                    </div>
                </div>
            </div>
        </body>
        </html>
        """;

        await SendEmailInternalAsync(toEmail, subject, htmlContent, cancellationToken);
    }

    private async Task SendEmailInternalAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp-relay.brevo.com";
        var smtpPortStr = _configuration["Email:SmtpPort"] ?? "587";
        var smtpUser = _configuration["Email:SmtpUsername"];
        var smtpPass = _configuration["Email:SmtpPassword"];
        if (string.IsNullOrWhiteSpace(smtpPass) || smtpPass.StartsWith("YOUR_"))
        {
            smtpPass = Environment.GetEnvironmentVariable("BREVO_SMTP_KEY")
                ?? Environment.GetEnvironmentVariable("Email__SmtpPassword");
        }
        var senderEmail = _configuration["Email:SenderEmail"] ?? "support@veval.edu.vn";
        var senderName = _configuration["Email:SenderName"] ?? "V-Eval Learning Platform";

        if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
        {
            _logger.LogWarning("[EMAIL SERVICE - MOCK MODE] Chưa cấu hình Email:SmtpUsername/Password trong appsettings.json. Ghi nhận gửi email ảo tới {ToEmail} | Subject: {Subject}",
                toEmail, subject);
            return;
        }

        int port = 587;
        if (int.TryParse(smtpPortStr, out var parsedPort))
        {
            port = parsedPort;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Timeout = 15000; // 15s timeout
            await client.ConnectAsync(smtpHost, port, SecureSocketOptions.Auto, cancellationToken);
            await client.AuthenticateAsync(smtpUser, smtpPass, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("[EMAIL SERVICE] Đã gửi thành công email tới {ToEmail} qua Brevo SMTP | Subject: {Subject}",
                toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EMAIL SERVICE ERROR] Gửi email tới {ToEmail} thất bại qua Brevo SMTP: {Message}",
                toEmail, ex.Message);
        }
    }
}

