# NHẬT KÝ KIỂM TRA TIẾN ĐỘ VẬN HÀNH (DAILY CHECK LOG) - V-EVAL IDENTITY SERVICE

## [18/09/2026] - Triển Khai Toàn Diện Clean Architecture, CQRS, Result Pattern & Multi-Schema PostgreSQL (iam & profile)
- **Thiết Kế & Triển Khai Domain Layer (`V-Eval-Identity_Service.Domain`)**:
  - Khởi tạo các thực thể thuộc schema `iam`: [`User.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/User.cs), [`Role.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/Role.cs), [`UserRole.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/UserRole.cs), [`RefreshToken.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/RefreshToken.cs), [`OtpVerification.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/OtpVerification.cs).
  - Khởi tạo thực thể thuộc schema `profile`: [`Student.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Student.cs) (quan hệ 1-1 trỏ sang `users.user_id`).
  - Định nghĩa bộ hằng số vai trò [`SystemRoles.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Constants/SystemRoles.cs) gồm 6 vai trò: `STUDENT`, `PARENT`, `TEACHER`, `ACADEMIC_MANAGER`, `ACADEMIC_DIRECTOR`, `ADMINISTRATOR`.

- **Thiết Kế & Triển Khai Application Layer (`V-Eval-Identity_Service.Application`)**:
  - Triển khai **Result Pattern** ([`Result.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Models/Result.cs), [`Error.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Models/Error.cs)) chuẩn hóa lỗi nghiệp vụ không quăng ngoại lệ unhandled.
  - Tích hợp **MediatR Pipeline Behavior** ([`ValidationBehavior.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Behaviors/ValidationBehavior.cs)) với FluentValidation tự động kiểm tra định dạng email, mật khẩu phức tạp (ít nhất 8 ký tự, chữ hoa, thường, số), số điện thoại di động Việt Nam.
  - Trừu tượng hóa giao diện Repositories & Unit of Work: [`IUnitOfWork.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IUnitOfWork.cs), [`IUserRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IUserRepository.cs), [`IRoleRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IRoleRepository.cs), [`IOtpRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IOtpRepository.cs), [`IStudentRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IStudentRepository.cs), [`IRefreshTokenRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IRefreshTokenRepository.cs).
  - Trừu tượng hóa hạ tầng: [`IJwtProvider.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IJwtProvider.cs), [`IPasswordHasher.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IPasswordHasher.cs), [`IOtpService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IOtpService.cs), [`ICurrentUserService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/ICurrentUserService.cs).
  - Triển khai toàn bộ Commands, Queries, Validators & Handlers cho:
    + Đăng ký tài khoản: [`RegisterUserCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/Register/RegisterUserCommand.cs).
    + Xác thực OTP: [`VerifyAccountCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/VerifyAccount/VerifyAccountCommand.cs).
    + Đăng nhập: [`LoginCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/Login/LoginCommand.cs).
    + Làm mới Token: [`RefreshTokenCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/RefreshToken/RefreshTokenCommand.cs).
    + Xem thông tin cá nhân: [`GetCurrentUserQuery.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Users/Queries/GetCurrentUser/GetCurrentUserQuery.cs).
    + Cập nhật hồ sơ: [`UpdateProfileCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Users/Commands/UpdateProfile/UpdateProfileCommand.cs).
    + Đổi mật khẩu: [`ChangePasswordCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Users/Commands/ChangePassword/ChangePasswordCommand.cs).

- **Thiết Kế & Triển Khai Infrastructure Layer (`V-Eval-Identity_Service.Infrastructure`)**:
  - Ánh xạ Multi-Schema PostgreSQL: [`AppDbContext.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/AppDbContext.cs) và [`AuthConfigurations.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Configurations/AuthConfigurations.cs) chỉ định rõ bảng vào schema `iam` và `profile`.
  - Triển khai **Cả Hai: Repository Pattern & Unit of Work**: [`UnitOfWork.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/UnitOfWork.cs), [`GenericRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/GenericRepository.cs), [`UserRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/UserRepository.cs), [`RoleRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/RoleRepository.cs), [`OtpRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/OtpRepository.cs), [`StudentRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/StudentRepository.cs), [`RefreshTokenRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs).
  - Triển khai các dịch vụ: [`PasswordHasher.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/PasswordHasher.cs) (BCrypt 11 work factor), [`JwtProvider.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/JwtProvider.cs) (JWT Token + Claims sub, email, roles, jti), [`OtpService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/OtpService.cs) (CSPs Random 6 số), [`CurrentUserService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/CurrentUserService.cs) (trích xuất claims từ `IHttpContextAccessor`).
  - Seed dữ liệu tự động: [`DbInitializer.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Seeds/DbInitializer.cs) tự động tạo schemas `iam`, `profile` và nạp đủ 6 vai trò cơ bản.

- **Thiết Kế & Triển Khai API Layer (`V-Eval-Identity_Service.API`)**:
  - Chuẩn hóa Base Controller: [`ApiControllerBase.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/Base/ApiControllerBase.cs) tự động chuyển đổi `Result<T>` sang mã HTTP Status Code tương ứng (200, 400, 401, 403, 404, 409).
  - Bộ điều khiển: [`AuthController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/AuthController.cs), [`UsersController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/UsersController.cs) (`[Authorize]`).
  - Xử lý lỗi toàn cục: [`GlobalExceptionHandlerMiddleware.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Middlewares/GlobalExceptionHandlerMiddleware.cs).
  - Pipeline & Cấu hình: [`Program.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Program.cs) cấu hình Authentication JWT Bearer, CORS, Swagger UI với nút Authorize Token tại root URL (`http://localhost:5155`).

- **Kiểm Thử Tích Hợp (E2E Integration Testing)**:
  - Khởi chạy EF Core Migration `20260917165927_InitialIdentityAndProfile` cập nhật thành công lên cơ sở dữ liệu Supabase PostgreSQL.
  - Thực thi kịch bản kiểm thử E2E tự động 9/9 bước đạt **100% Passed**:
    1. Đăng ký tài khoản học sinh (`POST /api/auth/register`) -> Thành công (IsActive = false, profile.students khởi tạo, sinh OTP 6 số).
    2. Đăng nhập khi chưa xác thực OTP (`POST /api/auth/login`) -> Bị chặn với mã **403 Forbidden**.
    3. Kích hoạt tài khoản với mã OTP (`POST /api/auth/verify-account`) -> Thành công (IsActive = true, Otp is_used = true).
    4. Đăng nhập sau khi kích hoạt (`POST /api/auth/login`) -> Cấp JWT Access Token và Refresh Token 7 ngày.
    5. Đọc thông tin hồ sơ (`GET /api/users/me`) -> Trả về thông tin IAM kèm Student Profile.
    6. Cập nhật hồ sơ học sinh (`PUT /api/users/me/profile`) -> Cập nhật thành công điểm mục tiêu, thời gian học, trường học.
    7. Đổi mật khẩu (`POST /api/users/me/change-password`) -> Kiểm tra mật khẩu cũ, băm và cập nhật mật khẩu mới.
    8. Đăng nhập lại với mật khẩu mới (`POST /api/auth/login`) -> Thành công.
    9. Cấp mới Access Token bằng Refresh Token (`POST /api/auth/refresh-token`) -> Thành công.
  - Biên dịch toàn bộ Solution `dotnet build` đạt **0 Error(s), 0 Warning(s)**.

---

## [15/09/2026] - Dockerize Identity Service & Chuẩn Hóa Docker Compose
- **Dockerfile Multi-Stage .NET 9**:
  - Khởi tạo `Dockerfile` chuẩn cho Identity Service (`V-Eval-Identity_Service.API`) với cổng `5001`.
  - Kết nối chung mạng nội bộ `veval_network` trong `docker-compose.yml`.

---

## [14/09/2026] - Chuẩn Hóa Cấu Hình Production & Git Security
- **Khởi Tạo `appsettings.example.json`**:
  - Tạo file cấu hình mẫu chứa `ConnectionStrings` và `JwtSettings`.
- **Bảo Mật Git Security**:
  - Cập nhật `.gitignore` ẩn tất cả file `appsettings.json` chứa mật khẩu cá nhân.
