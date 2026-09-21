# BÁO CÁO NGHIỆM THU & THẤU HIỂU KIẾN TRÚC IDENTITY SERVICE (IAM & PROFILE)

> **Dự án**: Hệ sinh thái Đánh giá Năng lực Thích ứng V-ACT 2026  
> **Người nghiệm thu & Đánh giá**: Developer & Senior AI Agent Pair-Programming  
> **Ngày nghiệm thu**: 18/09/2026  
> **Đánh giá tổng thể**: **Đạt chuẩn Enterprise Production / Clean Architecture / CQRS / Result Pattern / Multi-Schema PostgreSQL**

---

## 💡 NỘI DUNG TỔNG HỢP KIẾN THỨC & NGUYÊN LÝ ĐÃ THẤU HIỂU

### 1. Thấu hiểu Kiến Trúc 4 Tầng Clean Architecture & CQRS với MediatR
- **Domain Layer (Trọng tâm nghiệp vụ)**:
  - Là tầng thuần C# (không phụ thuộc bất kỳ framework hay thư viện bên ngoài nào).
  - Chứa các Entity mô hình hóa chính xác dữ liệu 2 schema `iam` ([`User.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/User.cs), [`Role.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/Role.cs), [`UserRole.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/UserRole.cs), [`RefreshToken.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/RefreshToken.cs), [`OtpVerification.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/OtpVerification.cs)) và `profile` ([`Student.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Student.cs)).
- **Application Layer (Điều phối nghiệp vụ & CQRS)**:
  - Áp dụng mô hình **CQRS (Command Query Responsibility Segregation)** thông qua thư viện `MediatR`:
    + **Commands** thay đổi trạng thái hệ thống: `RegisterUserCommand`, `VerifyAccountCommand`, `LoginCommand`, `RefreshTokenCommand`, `UpdateProfileCommand`, `ChangePasswordCommand`.
    + **Queries** truy vấn đọc dữ liệu mà không làm thay đổi trạng thái: `GetCurrentUserQuery`.
  - Tích hợp **Pipeline Behavior** ([`ValidationBehavior.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Behaviors/ValidationBehavior.cs)) chặn các request sai định dạng ngay trước khi vào Handler mà không cần viết lệnh kiểm tra lặp đi lặp lại trong từng Handler.
- **Infrastructure Layer (Hạ tầng & Kết nối bên ngoài)**:
  - Hiện thực hóa tất cả các Interface được khai báo tại tầng Application (PostgreSQL DbContext, Repositories, Unit of Work, BCrypt PasswordHasher, JWT Provider, OtpService).
- **API Layer (Cổng giao tiếp Web/Client)**:
  - Đóng vai trò là Presentation Layer nhận HTTP Request, chuyển vào MediatR Pipeline và trả kết quả HTTP Status Code tương ứng về client.

---

### 2. Thấu hiểu Result Pattern vs Ném Ngoại Lệ (Exception Throwing)
- **Hạn chế của việc quăng ngoại lệ (`throw new Exception`)**:
  - Ném exception trong luồng nghiệp vụ thông thường (như nhập sai mật khẩu, email đã tồn tại, OTP hết hạn) gây ra chi phí lớn về hiệu năng (Overhead do CLR phải tạo StackTrace và dọn dẹp bộ nhớ).
  - Làm code trở nên khó lường (Unpredictable control flow) vì người gọi không biết trước method có thể văng ra những exception nào.
- **Ưu điểm của Result Pattern ([`Result.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Models/Result.cs), [`Result<T>.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Models/Result.cs))**:
  - Trả về đối tượng tường minh: `Result.Success(value)` hoặc `Result.Failure(error)`.
  - Mã lỗi được phân loại rõ ràng qua enum `ErrorType` (`Validation`, `NotFound`, `Conflict`, `Unauthorized`, `Forbidden`).
  - [`ApiControllerBase.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/Base/ApiControllerBase.cs) tự động ánh xạ `ErrorType` sang mã HTTP Status Code chuẩn RFC (400, 401, 403, 404, 409) mà không cần cấu trúc `try-catch` cồng kềnh trong Controller.

---

### 3. Thấu hiểu Multi-Schema PostgreSQL trong EF Core 9
- **Tách biệt Logic & Quản trị dữ liệu**:
  - Schema `iam`: Đảm nhiệm định danh, vai trò, xác thực OTP và phiên đăng nhập (`users`, `roles`, `user_roles`, `refresh_tokens`, `otp_verifications`).
  - Schema `profile`: Đảm nhiệm hồ sơ mở rộng của người dùng (`students`).
- **Cấu hình Fluent API ([`AuthConfigurations.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Configurations/AuthConfigurations.cs))**:
  - Ánh xạ rõ ràng: `builder.ToTable("users", "iam");`, `builder.ToTable("students", "profile");`.
  - Quan hệ 1-1 giữa `iam.users` và `profile.students` sử dụng `StudentId` vừa là Khóa chính (PK), vừa là Khóa ngoại (FK) trỏ sang `users.user_id` với cơ chế xóa tầng `OnDelete(DeleteBehavior.Cascade)`.
  - Khi áp dụng Migration, EF Core tự động phát sinh lệnh `EnsureSchema("iam")` và `EnsureSchema("profile")` an toàn tuyệt đối.

---

### 4. Thấu hiểu Việc Triển Khai CẢ HAI: Repository Pattern & Unit of Work Pattern
- **Tại sao cần cả hai?**:
  - `DbContext` của EF Core vốn dĩ đã là một Unit of Work, và `DbSet` là Repository. Tuy nhiên, nếu tiêm trực tiếp `AppDbContext` vào tầng Application thì tầng Application sẽ bị gắn chặt (`tight coupling`) với EF Core, không thể viết Unit Test độc lập nếu không cài đặt thư viện EF Core.
  - **Repository Pattern (`IUserRepository`, `IRoleRepository`,...)**: Giúp che giấu chi tiết Linq/SQL, cung cấp các hàm nghiệp vụ rõ ràng như `GetByEmailWithRolesAsync`, `IsEmailUniqueAsync`.
  - **Unit of Work Pattern ([`IUnitOfWork.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IUnitOfWork.cs))**: Đảm bảo **Tính Nguyên Tử (Atomicity)** của giao dịch. Trong nghiệp vụ Đăng ký học sinh: vừa thêm `iam.users`, `iam.user_roles`, `profile.students`, vừa thêm `iam.otp_verifications`. `UnitOfWork.SaveChangesAsync()` đảm bảo tất cả cùng thành công hoặc cùng rollback nếu có bất kỳ lỗi nào xảy ra.

---

### 5. Thấu hiểu Bảo Mật Mật Khẩu (BCrypt) & Xác Thực Tài Khoản (OTP)
- **Băm mật khẩu an toàn với BCrypt ([`PasswordHasher.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/PasswordHasher.cs))**:
  - Sử dụng hệ số công (`Work Factor = 11`) chống lại các cuộc tấn công Brute-force và Rainbow Table bằng cách tự động gán Salt ngẫu nhiên trong chuỗi hash.
- **Chu trình Xác thực OTP 2 bước (`iam.otp_verifications`)**:
  - Bước 1 (Đăng ký): Tài khoản mới luôn được khởi tạo với `is_active = false`. Sinh mã OTP 6 số qua thuật toán mã hóa ngẫu nhiên an toàn `RandomNumberGenerator` và quy định thời hạn `ExpiresAt = DateTime.UtcNow.AddMinutes(10)`.
  - Bước 2 (Kích hoạt): Kiểm tra đúng mã OTP, chưa bị sử dụng (`is_used = false`) và còn hạn $\rightarrow$ Cập nhật `is_active = true` và đánh dấu `is_used = true` để ngăn chặn tấn công phát lại (Replay Attack).

---

### 6. Thấu hiểu Token JWT & Refresh Token Rotation
- **JWT Access Token ([`JwtProvider.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/JwtProvider.cs))**:
  - Chứa các Claims cốt lõi: `sub` (UserId), `email`, `name`, `roles` và `jti` (JWT ID duy nhất chống replay).
  - Ký bằng thuật toán `HmacSha256` với Secret Key tối thiểu 256 bits (32 bytes).
  - Thời hạn sống ngắn (15 - 60 phút) để hạn chế rủi ro lộ token.
- **Refresh Token (`iam.refresh_tokens`)**:
  - Chuỗi ngẫu nhiên dài 64 bytes sinh bởi `RandomNumberGenerator`, hạn sử dụng 7 ngày.
  - Khi client gọi `/api/auth/refresh-token`: Hệ thống kiểm tra token trong DB, lập tức **đánh dấu thu hồi token cũ (`is_revoked = true`)** và cấp một cặp Access Token + Refresh Token mới hoàn toàn (**Refresh Token Rotation**), đảm bảo an toàn tuyệt đối ngay cả khi token bị rò rỉ.

---

### 7. Thấu hiểu Cơ Chế Quên / Đặt Lại Mật Khẩu (OTP Verification) & Thu Hồi Phiên (Revoke Tokens)
- **Chu trình Quên Mật Khẩu 2 bước an toàn (`iam.otp_verifications`)**:
  - `POST /api/Auth/forgot-password`: Kiểm tra tài khoản tồn tại và đang hoạt động (`IsActive == true`). Tạo mã OTP 6 số bảo mật cao với `Type = "RESET_PASSWORD"`, thời hạn 10 phút.
  - `POST /api/Auth/reset-password`: So khớp OTP theo đúng `email`, `otpCode` và `Type = "RESET_PASSWORD"`. Đánh dấu `is_used = true` ngay khi xác thực để chống tấn công Replay Attack. Băm mật khẩu mới bằng BCrypt Work Factor 11.
- **Cơ chế Thu Hồi Toàn Bộ Phiên Đăng Nhập (`RevokeAllByUserIdAsync`)**:
  - Khi người dùng đặt lại mật khẩu, hệ thống tự động gọi `_refreshTokenRepository.RevokeAllByUserIdAsync(user.UserId)` để đánh dấu `is_revoked = true` cho mọi Refresh Token đang hoạt động của người dùng đó. Điều này đảm bảo tất cả các thiết bị/phiên cũ đều bị vô hiệu hóa, ngăn chặn kẻ gian chiếm đoạt tài khoản từ phiên đăng nhập trước đó.
- **Cơ chế Đăng Xuất (`LogoutCommand`)**:
  - `POST /api/Auth/logout`: Tiếp nhận `RefreshToken` từ client, đánh dấu `IsRevoked = true` trong CSDL `iam.refresh_tokens`. Khi token bị revoke, client không thể sử dụng token này để xin cấp Access Token mới thông qua `/api/Auth/refresh-token`.

---

### 8. Architectural Understanding: Campus Management, 6 Actor Profiles & Inter-Service gRPC
- **Campus & Multi-Actor Profiles Separation**:
  - `iam.campuses` acts as the physical and operational anchor for academic coordination (teachers and academic managers are linked via `campus_id`).
  - The 6 Actor profiles (`Student`, `Parent`, `ParentStudentRelation`, `Teacher`, `AcademicManager`, `AcademicDirector`, `Administrator`) cleanly partition specialized domain properties without cluttering the core `iam.users` table, ensuring strong single-responsibility and extensible identity modeling.
- **Inter-Service gRPC Protocol (`IdentityGrpc`)**:
  - High-performance binary transport over HTTP/2 eliminates JSON serialization overhead and ensures strongly-typed RPC contracts across Microservices.
  - `ValidateUserPermission`: Enables Gateway/Downstream services to verify active status and role claims.
  - `GetStudentProfileSummary`: Serves downstream placement & diagnostic workflows (Core Flow 1 in `Practice_Service`) by delivering critical target metrics and verified campus linkage.

---

## 📑 BẢNG NGHIỆM THU DANH MỤC FILE CODE IDENTITY SERVICE

| Nhóm Tầng Kiến Trúc | Tệp Code / Tài Liệu | Trạng Thái Nghiệm Thu |
| :--- | :--- | :---: |
| **Domain Entities (IAM)** | [`User.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/User.cs), [`Role.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/Role.cs), [`UserRole.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/UserRole.cs), [`RefreshToken.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/RefreshToken.cs), [`OtpVerification.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/OtpVerification.cs), [`Campus.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/Campus.cs) | **ĐẠT (100%)** |
| **Domain Entities (Profiles)** | [`Student.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Student.cs), [`Parent.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Parent.cs), [`ParentStudentRelation.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/ParentStudentRelation.cs), [`Teacher.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Teacher.cs), [`AcademicManager.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/AcademicManager.cs), [`AcademicDirector.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/AcademicDirector.cs), [`Administrator.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Administrator.cs) | **ĐẠT (100%)** |
| **Domain Constants** | [`SystemRoles.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Constants/SystemRoles.cs) (6 System Roles) | **ĐẠT (100%)** |
| **Application Common** | [`Result.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Models/Result.cs), [`Error.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Models/Error.cs), [`ValidationBehavior.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Behaviors/ValidationBehavior.cs) | **ĐẠT (100%)** |
| **Application Interfaces** | [`IUnitOfWork.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IUnitOfWork.cs), [`IUserRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IUserRepository.cs), [`IRoleRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IRoleRepository.cs), [`IOtpRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IOtpRepository.cs), [`IStudentRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IStudentRepository.cs), [`IRefreshTokenRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IRefreshTokenRepository.cs), [`ICampusRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/ICampusRepository.cs), [`IJwtProvider.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IJwtProvider.cs), [`IPasswordHasher.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IPasswordHasher.cs), [`IOtpService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/IOtpService.cs), [`ICurrentUserService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/ICurrentUserService.cs) | **ĐẠT (100%)** |
| **Application Features (Auth)** | [`RegisterUserCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/Register/RegisterUserCommand.cs), [`VerifyAccountCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/VerifyAccount/VerifyAccountCommand.cs), [`LoginCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/Login/LoginCommand.cs), [`RefreshTokenCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/RefreshToken/RefreshTokenCommand.cs), [`ForgotPasswordCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/ForgotPassword/ForgotPasswordCommand.cs), [`ResetPasswordCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/ResetPassword/ResetPasswordCommand.cs), [`LogoutCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/Logout/LogoutCommand.cs), [`AuthDtos.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/DTOs/AuthDtos.cs) | **ĐẠT (100%)** |
| **Application Features (Users & Campuses)** | [`GetCurrentUserQuery.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Users/Queries/GetCurrentUser/GetCurrentUserQuery.cs), [`UpdateProfileCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Users/Commands/UpdateProfile/UpdateProfileCommand.cs), [`ChangePasswordCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Users/Commands/ChangePassword/ChangePasswordCommand.cs), [`UserProfileDto.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Users/DTOs/UserProfileDto.cs), [`GetCampusesQuery.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Campuses/Queries/GetCampuses/GetCampusesQuery.cs), [`CampusDto.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Campuses/DTOs/CampusDto.cs) | **ĐẠT (100%)** |
| **Infrastructure Persistence** | [`AppDbContext.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/AppDbContext.cs), [`AuthConfigurations.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Configurations/AuthConfigurations.cs), [`DbInitializer.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Seeds/DbInitializer.cs), [`UnitOfWork.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/UnitOfWork.cs), [`GenericRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/GenericRepository.cs), [`UserRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/UserRepository.cs), [`RoleRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/RoleRepository.cs), [`OtpRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/OtpRepository.cs), [`StudentRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/StudentRepository.cs), [`RefreshTokenRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs), [`CampusRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/CampusRepository.cs) | **ĐẠT (100%)** |
| **Infrastructure Services** | [`PasswordHasher.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/PasswordHasher.cs), [`JwtProvider.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/JwtProvider.cs), [`OtpService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/OtpService.cs), [`CurrentUserService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Services/CurrentUserService.cs), [`DependencyInjection.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/DependencyInjection.cs) | **ĐẠT (100%)** |
| **API Layer** | [`ApiControllerBase.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/Base/ApiControllerBase.cs), [`AuthController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/AuthController.cs), [`UsersController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/UsersController.cs), [`CampusesController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/CampusesController.cs), [`IdentityGrpcService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs), [`GlobalExceptionHandlerMiddleware.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Middlewares/GlobalExceptionHandlerMiddleware.cs), [`Program.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Program.cs) | **ĐẠT (100%)** |
| **Protos Contract** | [`identity.proto`](file:///d:/Capstone/grpc/identity.proto) (`VEval.Grpc.Identity`) | **ĐẠT (100%)** |
