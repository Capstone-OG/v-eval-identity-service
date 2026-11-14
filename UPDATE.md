# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [15/11/2026] - Khắc Phục Lỗi Xác Thực Brevo SMTP (535 Authentication Failed), Bỏ Toàn Bộ Hardcode Khóa SMTP & Đồng Bộ Volume Cấu Hình

- **Loại Bỏ Hoàn Toàn Hardcode Khóa SMTP Trong [`BrevoSmtpEmailService.cs`](./V-Eval-Identity_Service.Infrastructure/Services/BrevoSmtpEmailService.cs) & [`appsettings.json`](./V-Eval-Identity_Service.API/appsettings.json)**:
  - Xóa bỏ toàn bộ chuỗi Base64 fallback hardcoded trong mã nguồn C#.
  - Dịch vụ email tự động đọc cấu hình theo thứ tự ưu tiên: `Email:SmtpPassword` trong cấu hình -> biến môi trường `BREVO_SMTP_KEY` -> biến môi trường `Email__SmtpPassword`. Nếu không được cấu hình, dịch vụ chuyển sang chế độ Mock mode an toàn và ghi log cảnh báo thay vì cố gửi với khóa mặc định.
  - Khôi phục giá trị an toàn `"SmtpPassword": "YOUR_BREVO_SMTP_KEY_OR_ENV_VAR"` trong `appsettings.json` tránh để lộ khóa bảo mật trên git repository.

- **Đồng Bộ Cấu Hình Trực Tiếp Vào Container Docker ([`docker-compose.yml`](../../docker-compose.yml))**:
  - Bổ sung volume mount `./All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/appsettings.json:/app/appsettings.json:ro` và `env_file: .env` cho container `v_eval_identity_service`.
  - Truyền biến môi trường `BREVO_SMTP_KEY` và `Email__SmtpPassword` trực tiếp từ file `.env` root vào container giúp nạp khóa SMTP tức thì khi khởi chạy container mà không cần rebuild Docker image.

- **Kiểm Thử & Nghiệm Thu**:
  - `dotnet build`: Biên dịch thành công 100% với .NET 9 SDK (`0 Error`).
  - Toàn bộ test case và cú pháp cấu hình docker-compose được xác thực hợp lệ.
