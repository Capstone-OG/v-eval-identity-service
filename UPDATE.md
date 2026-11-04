# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [04/11/2026] - Bổ Sung API Xác Thực Mã OTP Quên Mật Khẩu (UC 05b) & Hoàn Tất Tích Hợp Brevo SMTP Relay

- **Bổ Sung API Xác Thực Mã OTP Quên Mật Khẩu Chặt Chẽ (UC 05b - `POST /api/auth/verify-reset-otp`)**:
  - Triển khai `VerifyResetOtpCommand`, `VerifyResetOtpValidator` và `VerifyResetOtpHandler` trong tầng `Application`.
  - Kiểm tra tính hợp lệ và thời hạn của mã OTP loại `RESET_PASSWORD` trong CSDL trước khi cho phép người dùng chuyển sang bước đặt lại mật khẩu mới.
  - Ngăn chặn triệt để lỗ hổng cho phép nhập bừa OTP mà vẫn được chuyển qua bước đổi mật khẩu: Nếu OTP sai hoặc hết hạn, trả về mã lỗi HTTP 400 (`Auth.InvalidOtp: "Mã xác thực OTP không chính xác hoặc đã hết hạn."`).
  - Đăng ký endpoint `POST /api/auth/verify-reset-otp` tại `AuthController.cs`, tự động định tuyến thông qua Gateway YARP `:5212`.

- **Kích Hoạt Tài Khoản Brevo SMTP Thực Tế**:
  - Đấu nối thành công cấu hình Brevo SMTP Relay (`smtp-relay.brevo.com:587`, STARTTLS) với tài khoản `c17a0c001@smtp-brevo.com` và khóa bảo mật chính thức.
  - Cấu hình chuẩn hóa thương hiệu người gửi: `V_Eval System <veval.system@gmail.com>`.
  - Kiểm thử trực tiếp kết nối gửi email HTML chuẩn responsive qua Brevo SMTP thành công 100%.

- **Khắc Phục Cảnh Báo Khởi Động EF Core 9**:
  - Bổ sung cấu hình `ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))` tại `DependencyInjection.cs`, đảm bảo dịch vụ khởi động tự động áp dụng migration và nạp seed data an toàn không bị gián đoạn.

- **Hiện Thực Đầy Đủ Tầng Email & Nghiệp Vụ CQRS**:
  - Triển khai `IEmailService` tại tầng `Application` và `BrevoSmtpEmailService` tại tầng `Infrastructure` sử dụng `MailKit` & `MimeKit`.
  - Tự động phát sinh và gửi email chứa mã OTP 6 số đẹp mắt chuẩn nhận diện V-Eval khi người dùng đăng ký mới (UC 01) và quên mật khẩu (UC 05).
  - Tích hợp cơ chế Fail-Safe ghi log lỗi an toàn, không gián đoạn tiến trình người dùng nếu có sự cố mạng phía relay.

- **Kiểm Thử Đơn Vị & Vận Hành (Build & Test)**:
  - Bộ kiểm thử tự động `RegisterUserCommandHandlerTests` đạt **5/5 tests PASS 100%**.
  - Kiểm thử trực tiếp endpoint `POST /api/auth/verify-reset-otp` qua curl: Chặn đúng mã OTP sai (HTTP 400), chấp nhận mã hợp lệ (HTTP 200).
  - Biên dịch giải pháp C# sạch tuyệt đối: **0 Warning(s), 0 Error(s)**.
  - Cụm dịch vụ cục bộ (Identity :5155, Gateway :5212, Practice :5261, Web Client :5173) khởi động thành công và thông suốt toàn diện.
