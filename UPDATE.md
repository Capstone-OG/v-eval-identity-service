# Nhật Ký Cập Nhật (Update Log) - Identity Service

## [20/09/2026] - Hoàn Thiện Thực Thể Campus, 6 Actor Profiles, API Cơ Sở & Server gRPC Identity
- **UC 10 & UC 40: Quản Lý & Lựa Chọn Cơ Sở Đào Tạo (Campus)**:
  - Bổ sung thực thể [`Campus.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/Campus.cs) vào schema `iam.campuses`.
  - Cấu hình Fluent API bảng `campuses` và Seed 2 cơ sở mẫu (CS Thủ Đức và CS Quận 10).
  - Triển khai `ICampusRepository`, `CampusRepository` và cung cấp endpoint `GET /api/v1/campuses` qua [`CampusesController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/CampusesController.cs).
- **UC 35: Quản Lý 6 Actor Profiles (Schema `profile`)**:
  - Tạo trọn bộ 6 thực thể Actor Profile: [`Parent.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Parent.cs), [`ParentStudentRelation.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/ParentStudentRelation.cs), [`Teacher.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Teacher.cs), [`AcademicManager.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/AcademicManager.cs), [`AcademicDirector.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/AcademicDirector.cs), [`Administrator.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Administrator.cs).
  - Cấu hình Fluent API quan hệ 1-1 với `iam.users` (Cascade Delete) và quan hệ cơ sở `iam.campuses`.
  - Đăng ký đầy đủ `DbSet` trong `AppDbContext`.
- **UC 04: Chuẩn Hóa & Tách Biệt Hồ Sơ Dùng Chung (IAM) và Hồ Sơ Học Sinh (Students)**:
  - [`UsersController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/UsersController.cs): Chuyên trách thông tin tài khoản dùng chung (`GET /api/v1/users/me`, `PUT /api/v1/users/me/profile`, `POST /api/v1/users/me/change-password`). DTO `UserProfileDto` hoàn toàn trung lập, không chứa các trường riêng của học sinh.
  - [`StudentsController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/StudentsController.cs): Chuyên trách hồ sơ học sinh (`GET /api/v1/students/me/profile`, `PUT /api/v1/students/me/profile`) chuẩn hóa thang điểm `TargetScore` tối đa 1200 điểm (theo chuẩn kỳ thi ĐGNL ĐHQG-HCM V-ACT) và cơ sở đào tạo `CampusId`.
- **Liên Dịch Vụ gRPC (IdentityGrpcService)**:
  - Hợp đồng [`identity.proto`](file:///d:/Capstone/grpc/identity.proto) dùng chung giữa các microservices.
  - Triển khai server gRPC [`IdentityGrpcService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs) cung cấp 2 RPC: `ValidateUserPermission` (kiểm tra quyền/trạng thái) và `GetStudentProfileSummary` (cung cấp CampusId và TargetScore cho Practice Service).
  - Cấu hình biên dịch Protobuf và đăng ký `MapGrpcService` trong `Program.cs`.
- **Kiểm Thử & Build**:
  - Giải pháp `V-Eval-Identity_Service.sln` biên dịch sạch 100% (`dotnet build` 0 Error, 0 Warning).

## [18/09/2026] - Bổ Sung Trọn Bộ 3 API: Quên Mật Khẩu (OTP), Đặt Lại Mật Khẩu & Đăng Xuất (Logout)
- **UC 05: Quên Mật Khẩu (`POST /api/Auth/forgot-password`)**:
  - Triển khai `ForgotPasswordCommand`, kiểm tra tài khoản hợp lệ, tự động sinh mã OTP 6 số lưu vào `iam.otp_verifications` với `Type = "RESET_PASSWORD"`, thời hạn 10 phút.
  - Trả về `ForgotPasswordResponseDto` (kèm mã OTP để test môi trường Dev).
- **UC 06: Đặt Lại Mật Khẩu (`POST /api/Auth/reset-password`)**:
  - Triển khai `ResetPasswordCommand`, so khớp OTP `RESET_PASSWORD` chưa sử dụng, băm mật khẩu mới bằng BCrypt Work Factor 11, đánh dấu OTP đã dùng (`is_used = true`).
  - Tự động kích hoạt cơ chế bảo mật `_refreshTokenRepository.RevokeAllByUserIdAsync(userId)`: thu hồi toàn bộ Refresh Token cũ của User, buộc đăng xuất trên mọi thiết bị và yêu cầu đăng nhập lại với mật khẩu mới.
- **UC 07: Đăng Xuất Hệ Thống (`POST /api/Auth/logout`)**:
  - Triển khai `LogoutCommand`, tiếp nhận `RefreshToken`, đánh dấu `is_revoked = true` trong database `iam.refresh_tokens`, làm mất hiệu lực chìa khóa phiên làm việc, ngăn không cho cấp mới Access Token.
- **Biên Dịch & Kiểm Thử**:
  - Giải pháp `V-Eval-Identity_Service` biên dịch sạch 100% (`dotnet build` 0 Error, 0 Warning).

## [18/09/2026] - Triển Khai Toàn Diện Authentication & User Profile, Chuẩn Hóa Bộ Docs & Script Push Độc Lập
- **Chuẩn Hóa Bộ Tài Liệu Service (`docs/`)**:
  - Đổi tên `docs/daily_check_log.md` -> [`docs/daily.md`](file:///e:/CapStone/All%20Services/V-Eval-Identity_Service/docs/daily.md).
  - Đổi tên `docs/daily_process_and_planning.md` -> [`docs/process.md`](file:///e:/CapStone/All%20Services/V-Eval-Identity_Service/docs/process.md).
  - Đổi tên `docs/nghiem_thu_va_thau_hieu_kien_truc.md` -> [`docs/architecture_acceptance.md`](file:///e:/CapStone/All%20Services/V-Eval-Identity_Service/docs/architecture_acceptance.md).
- **Phát Hành Công Cụ Push Độc Lập (`Scripts/push.bat`)**:
  - Khởi tạo script [`Scripts/push.bat`](file:///e:/CapStone/All%20Services/V-Eval-Identity_Service/Scripts/push.bat) hỗ trợ Push trên nhánh hiện tại, chọn nhánh đã có qua Menu đánh số, hoặc tạo nhánh mới tự động.
  - Tích hợp tự động kiểm tra đồng bộ lịch sử Git với Remote, tự động pull code khi bi cham (behind) và đưa ra **Cảnh báo Đỏ (Red Warning)** ngắt quy trình khi bị xung đột lịch sử (Conflict/Diverged).
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
