# KIẾN TRÚC & BẢNG THEO DÕI TIẾN ĐỘ CHỦ THỂ (PROCESS & PLANNING) - V-EVAL IDENTITY SERVICE

---

## PHẦN 1: KIẾN TRÚC DỊCH VỤ & CÁC THÀNH PHẦN CẦN TRIỂN KHAI

### 1. Kiến Trúc Clean Architecture 4 Tầng & CQRS (MediatR)
- **Cổng Dịch Vụ**: `5001` (HTTP) / Container `v_eval_identity_service`.
- **Phân Tầng**:
  - `Domain`: Entities (`User`, `Role`, `UserRole`, `RefreshToken`, `OtpVerification`, `Student`), Enums, Interfaces.
  - `Application`: Features CQRS (Commands, Queries, Handlers, Validators), Result Pattern (`Result<T>`), Pipeline Behaviors (`ValidationBehavior`).
  - `Infrastructure`: Multi-Schema EF Core 9 `AppDbContext` (`iam` & `profile`), Repositories, Unit of Work, Services (`JwtProvider`, `PasswordHasher`, `OtpService`).
  - `API`: Controllers (`AuthController`, `UsersController`), Middlewares (`GlobalExceptionHandlerMiddleware`), Swagger UI.

### 2. Sơ Đồ CSDL Multi-Schema PostgreSQL
- **Schema `iam`**:
  - `users`: Thông tin tài khoản chính (`user_id`, `email`, `password_hash`, `full_name`, `phone`, `is_active`).
  - `roles`: Danh mục 6 vai trò hệ thống (`STUDENT`, `PARENT`, `TEACHER`, `ACADEMIC_MANAGER`, `ACADEMIC_DIRECTOR`, `ADMINISTRATOR`).
  - `user_roles`: Bảng nối quan hệ N-N giữa User và Role.
  - `refresh_tokens`: Lưu vết Refresh Token kèm thời gian hết hạn và trạng thái thu hồi.
  - `otp_verifications`: Mã OTP 6 số kích hoạt tài khoản có hạn 10 phút.
- **Schema `profile`**:
  - `students`: Hồ sơ học sinh (`target_score`, `daily_study_time_minutes`, `school_name`) trỏ 1-1 tới `iam.users`.

---

## PHẦN 2: BẢNG THEO DÕI TIẾN ĐỘ CHI TIẾT THEO TỪNG MỤC (PROGRESS MATRIX)

| STT | Hạng Mục / Chức Năng | Vị Trí Triển Khai trong Code | Trạng Thái | Tiến Độ (%) | Ghi Chú Chi Tiết |
| :---: | :--- | :--- | :---: | :---: | :--- |
| 1 | **Clean Architecture 4 Tầng** | Entire Solution | 🟢 Hoàn thành | 100% | `Domain`, `Application`, `Infrastructure`, `API` |
| 2 | **Multi-Schema EF Core** | `Infrastructure/Persistence/` | 🟢 Hoàn thành | 100% | Schema `iam` và `profile` chạy trên Supabase |
| 3 | **Repository & Unit of Work** | `Application/Common/Interfaces/` | 🟢 Hoàn thành | 100% | `IUnitOfWork`, `IUserRepository`, `IStudentRepository`... |
| 4 | **UC 01: Đăng ký tài khoản** | `Features/Auth/Commands/Register/` | 🟢 Hoàn thành | 100% | Băm BCrypt, tạo Student profile, sinh OTP DB |
| 5 | **UC 02: Xác thực OTP** | `Features/Auth/Commands/VerifyAccount/` | 🟢 Hoàn thành | 100% | Khóa tài khoản chưa OTP, kích hoạt `is_active=true` |
| 6 | **UC 03: Đăng nhập & JWT** | `Features/Auth/Commands/Login/` | 🟢 Hoàn thành | 100% | Cấp cặp Access Token + Refresh Token 7 ngày |
| 7 | **Làm mới Token** | `Features/Auth/Commands/RefreshToken/` | 🟢 Hoàn thành | 100% | Cấp cặp token mới khi Access Token hết hạn |
| 8 | **UC 04: Quản lý Hồ sơ** | `Features/Users/` | 🟢 Hoàn thành | 100% | `GetCurrentUser`, `UpdateProfile`, `ChangePassword` |
| 9 | **Kiểm thử E2E 9/9 Steps** | Test Execution Log | 🟢 Hoàn thành | 100% | Đạt 100% Passed không có lỗi |
| 10 | **Script Push Độc Lập** | `Scripts/push.bat` | 🟢 Hoàn thành | 100% | Hỗ trợ 3 chế độ push kèm kiểm tra lịch sử |
| 11 | **OAuth2 (Google/Facebook)** | `Features/Auth/Commands/OAuth/` | 🟡 Đang chờ | 0% | Sẽ phát triển ở giai đoạn sau |
