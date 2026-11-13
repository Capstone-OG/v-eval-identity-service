# KIẾN TRÚC & BẢNG THEO DÕI TIẾN ĐỘ CHỦ THỂ (PROCESS & PLANNING) - V-EVAL IDENTITY SERVICE

---

## PHẦN 1: KIẾN TRÚC DỊCH VỤ & CÁC THÀNH PHẦN CẦN TRIỂN KHAI

### 1. Kiến Trúc Clean Architecture 4 Tầng, CQRS (MediatR) & gRPC Server
- **Cổng Dịch Vụ**: `5001` (HTTP / gRPC) / Container `v_eval_identity_service`.
- **Phân Tầng**:
  - `Domain`: 
    - Schema `iam`: `Campus`, `User`, `Role`, `UserRole`, `RefreshToken`, `OtpVerification`.
    - Schema `profile`: 6 Actor Profiles gồm `Student`, `Parent`, `ParentStudentRelation`, `Teacher`, `AcademicManager`, `AcademicDirector`, `Administrator`.
    - Enums, Interfaces, Hằng số vai trò (`SystemRoles`).
  - `Application`: Features CQRS (Commands, Queries, Handlers, Validators), Result Pattern (`Result<T>`), Pipeline Behaviors (`ValidationBehavior`).
    + Commands: `RegisterUserCommand`, `VerifyAccountCommand`, `LoginCommand`, `RefreshTokenCommand`, `ForgotPasswordCommand`, `ResetPasswordCommand`, `LogoutCommand`, `UpdateProfileCommand` (thang điểm 1200), `ChangePasswordCommand`.
    + Queries: `GetCurrentUserQuery`, `GetCampusesQuery`.
  - `Infrastructure`: Multi-Schema EF Core 9 `AppDbContext` (`iam` & `profile`), Repositories (`UserRepository`, `RoleRepository`, `OtpRepository`, `StudentRepository`, `RefreshTokenRepository`, `CampusRepository`), Unit of Work, Services (`JwtProvider`, `PasswordHasher`, `OtpService`, `CurrentUserService`).
  - `API`: Controllers (`AuthController`, `UsersController`, `CampusesController`), gRPC Services (`IdentityGrpcService`), Middlewares (`GlobalExceptionHandlerMiddleware`), Swagger UI.
  - `Protos`: `grpc/identity.proto` khai báo các RPC dùng cho liên dịch vụ.

### 2. Sơ Đồ CSDL Multi-Schema PostgreSQL (Đồng bộ với SQL.sql)
- **Schema `iam`**:
  - `campuses`: Quản lý danh mục cơ sở (`campus_id`, `code`, `name`, `address`, `phone`, `is_active`, `created_at`).
  - `users`: Thông tin tài khoản chính (`user_id`, `email`, `password_hash`, `full_name`, `phone`, `is_active`).
  - `roles`: Danh mục 6 vai trò hệ thống (`STUDENT`, `PARENT`, `TEACHER`, `ACADEMIC_MANAGER`, `ACADEMIC_DIRECTOR`, `ADMINISTRATOR`).
  - `user_roles`: Bảng nối quan hệ N-N giữa User và Role.
  - `refresh_tokens`: Lưu vết Refresh Token kèm thời gian hết hạn và trạng thái thu hồi.
  - `otp_verifications`: Mã OTP 6 số kích hoạt tài khoản / đặt lại mật khẩu có hạn 10 phút.
- **Schema `profile`**:
  - `students`: Hồ sơ học sinh (`target_score` thang 1200, `campus_id`, `exam_date`, `study_hours_day`, `school_name`) trỏ 1-1 tới `iam.users` và N-1 tới `iam.campuses`.
  - `parents`: Hồ sơ phụ huynh (`parent_id`, `phone_work`) trỏ 1-1 tới `iam.users`.
  - `parent_student_relations`: Bảng liên kết giữa phụ huynh và học sinh (`relation_id`, `parent_id`, `student_id`).
  - `teachers`: Hồ sơ giáo viên (`teacher_id`, `campus_id`, `specialty`, `bio`) trỏ 1-1 tới `iam.users` và N-1 tới `iam.campuses`.
  - `academic_managers`: Hồ sơ điều phối viên học vụ cơ sở (`manager_id`, `campus_id`).
  - `academic_directors`: Hồ sơ giám đốc học thuật (`director_id`).
  - `administrators`: Hồ sơ quản trị viên hệ thống (`admin_id`).

---

## PHẦN 2: BẢNG THEO DÕI TIẾN ĐỘ CHI TIẾT THEO TỪNG MỤC (PROGRESS MATRIX)

| STT | Hạng Mục / Chức Năng | Vị Trí Triển Khai trong Code | Trạng Thái | Tiến Độ (%) | Ghi Chú Chi Tiết |
| :---: | :--- | :--- | :---: | :---: | :--- |
| 1 | **Clean Architecture 4 Tầng** | Entire Solution | 🟢 Hoàn thành | 100% | `Domain`, `Application`, `Infrastructure`, `API` |
| 2 | **Multi-Schema EF Core** | `Infrastructure/Persistence/` | 🟢 Hoàn thành | 100% | Schema `iam` và `profile` chạy trên Supabase |
| 3 | **Repository & Unit of Work** | `Application/Common/Interfaces/` | 🟢 Hoàn thành | 100% | `IUnitOfWork`, `IUserRepository`, `IStudentRepository`, `IRefreshTokenRepository`, `ICampusRepository` |
| 4 | **UC 01: Đăng ký tài khoản** | `Features/Auth/Commands/Register/` | 🟢 Hoàn thành | 100% | Băm BCrypt, tạo Student profile, sinh OTP DB |
| 5 | **UC 02: Xác thực OTP** | `Features/Auth/Commands/VerifyAccount/` | 🟢 Hoàn thành | 100% | Khóa tài khoản chưa OTP, kích hoạt `is_active=true` |
| 6 | **UC 03: Đăng nhập & JWT** | `Features/Auth/Commands/Login/` | 🟢 Hoàn thành | 100% | Cấp cặp Access Token + Refresh Token 7 ngày |
| 7 | **Làm mới Token** | `Features/Auth/Commands/RefreshToken/` | 🟢 Hoàn thành | 100% | Cấp cặp token mới khi Access Token hết hạn |
| 8 | **UC 04: Quản lý Hồ sơ** | `Features/Users/` | 🟢 Hoàn thành | 100% | `GetCurrentUser`, `UpdateProfile` (thang điểm 1200, auto-create Student), `ChangePassword` |
| 9 | **UC 05: Quên Mật Khẩu (OTP)** | `Features/Auth/Commands/ForgotPassword/` | 🟢 Hoàn thành | 100% | Kiểm tra email, sinh OTP `RESET_PASSWORD` 10 phút |
| 10 | **UC 06: Đặt Lại Mật Khẩu** | `Features/Auth/Commands/ResetPassword/` | 🟢 Hoàn thành | 100% | So khớp OTP, hash mật khẩu mới, revoke toàn bộ token cũ |
| 11 | **UC 07: Đăng Xuất (Logout)** | `Features/Auth/Commands/Logout/` | 🟢 Hoàn thành | 100% | Thu hồi Refresh Token (`is_revoked = true`) vô hiệu hóa phiên |
| 12 | **UC 10 & UC 40: Quản lý & Chọn Cơ sở** | `Features/Campuses/`, `CampusesController.cs` | 🟢 Hoàn thành | 100% | `Campus` entity, `ICampusRepository`, `GET /api/v1/campuses` |
| 13 | **UC 35: Quản lý 6 Actor Profiles** | `Domain/Entities/Profiles/`, `AuthConfigurations.cs` | 🟢 Hoàn thành | 100% | Hoàn tất 6 hồ sơ Actor: `Student`, `Parent`, `Relation`, `Teacher`, `Manager`, `Director`, `Admin` |
| 14 | **gRPC Service (IdentityGrpc)** | `API/Services/IdentityGrpcService.cs` | 🟢 Hoàn thành | 100% | Triển khai `ValidateUserPermission` & `GetStudentProfileSummary` |
| 15 | **Kiểm thử E2E & Build Solution** | Test Execution Log | 🟢 Hoàn thành | 100% | `dotnet build` đạt 0 Error, 0 Warning |
| 16 | **Script Push Độc Lập** | `Scripts/push.bat` | 🟢 Hoàn thành | 100% | Hỗ trợ 3 chế độ push kèm kiểm tra lịch sử |
| 17 | **Cấp Phát Tài Khoản Trực Tiếp (IAM)** | `Features/Users/Commands/ProvisionUser/` | 🟢 Hoàn thành | 100% | `POST /api/v1/users/provision`: Cấp tài khoản trực tiếp 6 vai trò, kích hoạt ngay không OTP |
| 18 | **Quản Lý & Khóa/Mở Tài Khoản** | `Features/Users/Queries/GetUsers/`, `ToggleUserStatus/` | 🟢 Hoàn thành | 100% | `GET /api/v1/users` (phân trang, lọc vai trò, cơ sở), `PATCH /api/v1/users/{id}/toggle-status` |
| 19 | **OAuth2 (Google/Facebook)** | `Features/Auth/Commands/OAuth/` | 🟡 Đang chờ | 0% | Sẽ phát triển ở giai đoạn sau |
| 20 | **Chuẩn Hóa Cấu Hình & Centralized Config** | `V-Eval-Identity_Service.API/appsettings.example.json` | 🟢 Hoàn thành | 100% | Khớp cấu trúc Jwt/Kestrel, hỗ trợ sync_config.bat từ System-Repo |
| 21 | **Tối Ưu Đăng Ký Liên Kết Cơ Sở & Bảo Vệ Khóa Ngoại** | `Features/Auth/Commands/Register/RegisterUserCommand.cs` | 🟢 Hoàn thành | 100% | Bổ sung `CampusId`, logic guard tự động ánh xạ cơ sở hợp lệ, tạo hồ sơ Phụ huynh `Parents` |
| 22 | **Phân Hệ Brevo SMTP Email Service & Mẫu Thư HTML OTP V-Eval** | `Application/Common/Interfaces/IEmailService.cs`, `Infrastructure/Services/BrevoSmtpEmailService.cs` | 🟢 Hoàn thành | 100% | Tích hợp MailKit/MimeKit 4.17.0, Brevo SMTP Relay, template HTML V-Eval, đấu nối UC 01 và UC 05 (5/5 tests pass) |
| 23 | **UC 05b: API Xác Thực Mã OTP Quên Mật Khẩu Trước Khi Đổi Mật Khẩu** | `Features/Auth/Commands/VerifyResetOtp/`, `AuthController.cs` | 🟢 Hoàn thành | 100% | `POST /api/auth/verify-reset-otp`: Xác thực mã OTP loại `RESET_PASSWORD` với CSDL trước khi chuyển sang bước 3; trả về HTTP 400 chặn đứng nếu nhập sai hoặc hết hạn |
| 24 | **UC 01a: Kiểm Tra Tính Hợp Lệ Đa Tầng & Bản Ghi DNS MX Của Email Đăng Ký** | `Common/Validators/EmailValidationHelper.cs`, `Features/Auth/Queries/ValidateEmail/ValidateEmailQuery.cs`, `RegisterUserCommand.cs` | 🟢 Hoàn thành | 100% | Kiểm tra cú pháp RFC 5322, quy tắc Gmail, chặn chuỗi lặp giả mạo (`22222`), chặn email ảo 10 phút, gợi ý sửa sai chính tả tên miền và đối soát bản ghi DNS MX trực tuyến (`GET /api/auth/validate-email`) |
| 25 | **UC 41: Hệ Thống Liên Kết Phụ Huynh - Học Sinh 3 Phương Thức (Mã Code `VE-XXXXXX` / PIN 6 Số, Link & QR, Xác Nhận 2 Chiều Trên Web)** | `Features/Users/ParentLinking/ParentLinkingFeatures.cs`, `UserRepository.cs`, `UsersController.cs` | 🟢 Hoàn thành | 100% | `GET /api/v1/users/me/parent-link-overview`, `POST /api/v1/users/parent-link/connect`, `POST /api/v1/users/parent-link/request`, `POST /api/v1/users/me/parent-link-requests/{id}/respond` ghi trực tiếp vào `v_eval_identity.ParentStudentRelations` không phụ thuộc SMTP |
| 26 | **Tích Hợp Kiểm Tra Email Đa Tầng DNS MX Vào Cấp Phát Tài Khoản (IAM Provisioning) & Xử Lý Null CampusId** | `Features/Users/Commands/ProvisionUser/ProvisionUserCommand.cs`, `Student.cs`, `IdentityGrpcService.cs` | 🟢 Hoàn thành | 100% | Bổ sung `EmailValidationHelper` vào lệnh cấp tài khoản (chống typo domain `mail.com` -> `gmail.com`, chặn email ảo, kiểm tra DNS MX), chuyển `Student.CampusId` sang `Guid?` triệt tiêu lỗi InvalidCastException 500 khi tải danh sách người dùng |
| 27 | **Phân Hệ Quản Lý Tài Khoản Ngừng Hoạt Động (Soft Delete / Inactive) & Thống Kê Số Lượng Realtime** | `Features/Users/Queries/GetUsers/GetUsersQuery.cs`, `UserRepository.cs`, `UsersController.cs`, `ToggleUserStatusCommand.cs` | 🟢 Hoàn thành | 100% | Bổ sung lọc `bool? isActive` vào `GetUsersQuery`, trả về `ActiveCount` và `InactiveCount`, nâng cấp `ToggleUserStatusCommand` hỗ trợ `targetStatus` tùy chọn, bảo đảm nguyên tắc Soft Delete bảo toàn 100% dữ liệu bài thi và lịch sử học tập |
| 28 | **Chuyển Toàn Bộ Nhánh Làm Việc Sang Main, Cấu Hình Kestrel ListenAnyIP & Đóng Gói Docker Container Cho Hệ Thống V-Eval** | `Program.cs`, `Dockerfile`, `docker-compose.yml` | 🟢 Hoàn thành | 100% | Cấu hình `ListenAnyIP` cho cổng REST (5155) và gRPC (5156) thay thế `ListenLocalhost`, mở cổng trong Dockerfile và ánh xạ mạng Docker Compose, biên dịch và triển khai phục vụ endpoint `https://api.pickic.io.vn/api/v1/campuses` thành công 100% |
