# Nhật Ký Cập Nhật (Update Log) - Identity Service

## [18/09/2026] - Triển Khai Toàn Diện Authentication & User Profile (Clean Architecture & Multi-Schema PostgreSQL)
- **Kiến Trúc Clean Architecture 4 Tầng & CQRS (MediatR)**:
  - Tách biệt rõ ràng 4 tầng: `Domain`, `Application`, `Infrastructure`, `API`.
  - Triển khai CQRS thông qua MediatR, kết hợp `ValidationBehavior` Pipeline và FluentValidation tự động kiểm tra định dạng email, mật khẩu phức tạp, số điện thoại Việt Nam.
  - Chuẩn hóa luồng trả lời qua **Result Pattern** (`Result<T>`, `Error`, `ErrorType`), tuyệt đối không để lọt unhandled exceptions.
- **Tầng Dữ Liệu Multi-Schema PostgreSQL (EF Core 9)**:
  - Cấu hình 2 schema độc lập: `iam` (`users`, `roles`, `user_roles`, `refresh_tokens`, `otp_verifications`) và `profile` (`students`).
  - Áp dụng cấu hình Fluent API chuẩn mực cho toàn bộ quan hệ 1-1, 1-N và N-N.
  - Triển khai **Cả Hai: Repository Pattern & Unit of Work Pattern** (`IUnitOfWork`, `IUserRepository`, `IRoleRepository`, `IOtpRepository`, `IStudentRepository`, `IRefreshTokenRepository`).
  - Tự động nạp Seed Data cho **6 Roles hệ thống**: `STUDENT`, `PARENT`, `TEACHER`, `ACADEMIC_MANAGER`, `ACADEMIC_DIRECTOR`, `ADMINISTRATOR`.
- **Triển Khai Đầy Đủ 4 Use Cases**:
  - **UC 01: Đăng ký tài khoản (`RegisterUserCommand`)**: Băm mật khẩu bằng BCrypt, tạo User (`is_active = false`), tự động khởi tạo hồ sơ `profile.students`, sinh OTP 6 số lưu vào DB.
  - **UC 02: Xác thực tài khoản (`VerifyAccountCommand`)**: So khớp OTP trong 10 phút, kích hoạt tài khoản (`is_active = true`), đánh dấu OTP đã dùng.
  - **UC 03: Đăng nhập hệ thống (`LoginCommand`)**: Kiểm tra credentials, chặn tài khoản chưa kích hoạt (403), cấp JWT Access Token và Refresh Token 7 ngày.
  - **Làm mới Token (`RefreshTokenCommand`)**: Xác thực và cấp mới cặp Token.
  - **UC 04: Quản lý hồ sơ cá nhân**: Xem thông tin (`GetCurrentUserQuery`), Cập nhật hồ sơ học sinh (`UpdateProfileCommand`), Đổi mật khẩu an toàn (`ChangePasswordCommand`).
- **Swagger UI & Bảo Mật JWT Bearer**:
  - Tích hợp nút Authorize trên Swagger UI tại root URL (`http://localhost:5155`).
  - Toàn bộ 9/9 bước kiểm thử tích hợp E2E đã chạy thành công 100%.

## [15/09/2026] - Dockerize Identity Service & Chuẩn Hóa Docker Compose
- **Dockerfile Multi-Stage .NET 9**:
  - Khởi tạo `Dockerfile` chuẩn cho Identity Service (`V-Eval-Identity_Service.API`) với cổng `5001`.
  - Kết nối chung mạng nội bộ `veval_network` trong `docker-compose.yml`.

## [14/09/2026] - Chuẩn Hóa Cấu Hình Production & Git Security
- **Khởi Tạo `appsettings.example.json`**:
  - Tạo file cấu hình mẫu chứa `ConnectionStrings` và `JwtSettings`.
- **Bảo Mật Git Security**:
  - Cập nhật `.gitignore` ẩn tất cả file `appsettings.json` chứa mật khẩu cá nhân.
