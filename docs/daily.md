# NHẬT KÝ KIỂM TRA TIẾN ĐỘ VẬN HÀNH (DAILY CHECK LOG) - V-EVAL IDENTITY SERVICE

## [07/11/2026] - Tích Hợp Kiểm Tra Email Đa Tầng (DNS MX & Chống Typo Tên Miền) Vào Phân Hệ Cấp Phát Tài Khoản (IAM Provisioning) & Khắc Phục Triệt Để Lỗi Null CampusId Học Sinh

- **Triển Khai Phân Hệ Quản Lý Tài Khoản Ngừng Hoạt Động (Soft Delete / Inactive) & Thống Kê Số Lượng Hoạt Động ([`GetUsersQuery.cs`](../V-Eval-Identity_Service.Application/Features/Users/Queries/GetUsers/GetUsersQuery.cs), [`UserRepository.cs`](../V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/UserRepository.cs), [`UsersController.cs`](../V-Eval-Identity_Service.API/Controllers/UsersController.cs))**:
  - Bổ sung tham số lọc `bool? isActive` vào truy vấn `GetUsersQuery` và endpoint `GET /api/v1/users`, cho phép phân tách rạch ròi giữa tài khoản đang hoạt động (`isActive = true`) và tài khoản đã ngừng hoạt động/ẩn (`isActive = false`).
  - Nâng cấp `PagedUsersResult` trả về đồng thời `ActiveCount` và `InactiveCount` theo thời gian thực từ cơ sở dữ liệu PostgreSQL.
  - Nâng cấp lệnh `ToggleUserStatusCommand` (`PATCH /api/v1/users/{id}/toggle-status`) hỗ trợ tham số `targetStatus` tùy chọn, cho phép chủ động đặt trạng thái kích hoạt hoặc ngừng hoạt động có kiểm soát.
  - Bảo đảm nguyên tắc Soft Delete: Không xóa cứng dữ liệu khỏi bảng `Users`, bảo toàn 100% lịch sử nộp bài thi, giao dịch học phí và liên kết phụ huynh.
- **Tích Hợp Bộ Kiểm Tra Email Đa Tầng Vào Cấp Phát Tài Khoản ([`ProvisionUserCommand.cs`](../V-Eval-Identity_Service.Application/Features/Users/Commands/ProvisionUser/ProvisionUserCommand.cs))**:
  - Tích hợp `EmailValidationHelper.ValidateEmailComprehensiveAsync` vào lệnh `ProvisionUserCommand` trước khi lưu vào CSDL.
  - Bổ sung phát hiện lỗi gõ nhầm tên miền `mail.com` thành `gmail.com` trong [`EmailValidationHelper.cs`](../V-Eval-Identity_Service.Application/Common/Validators/EmailValidationHelper.cs) (`CommonDomainTypos`).
  - Chặn đứng các trường hợp nhập sai email, tên miền không có bản ghi DNS MX, dịch vụ email rác/ảo 10 phút và chuỗi ký tự lặp giả mạo (`22222`).
- **Khắc Phục Triệt Để Lỗi Null `CampusId` Khi Tải Danh Sách Người Dùng ([`Student.cs`](../V-Eval-Identity_Service.Domain/Entities/Profiles/Student.cs), [`IdentityGrpcService.cs`](../V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs))**:
  - Chuyển trường `CampusId` trong thực thể `Student` sang dạng nullable (`Guid?`) cho phép EF Core ánh xạ an toàn các bản ghi học sinh cũ chưa gán cơ sở.
  - Cập nhật logic xử lý an toàn trong `IdentityGrpcService.cs`, loại bỏ hoàn toàn ngoại lệ `InvalidCastException: Column 'campus_id' is null` gây lỗi HTTP 500 khi gọi `GET /api/v1/users`.
  - Cập nhật 5 bản ghi học sinh thiếu cơ sở trong CSDL PostgreSQL sang cơ sở mặc định `CS_THU_DUC` (`3fa85f64-5717-4562-b3fc-2c963f66afa6`).
- **Kiểm Thử & Biên Dịch**:
  - Biên dịch .NET 9 SDK đạt chuẩn 100% sạch (0 Warning, 0 Error).
  - Toàn bộ 5/5 kiểm thử xUnit đạt Passed 100%.

---

- **Triển Khai Bộ Kiểm Tra Email Đa Tầng (`EmailValidationHelper.cs`)**:
  - Kiểm tra cú pháp chuẩn RFC 5322, cấu trúc TLD quốc tế và quy tắc riêng của Google Gmail (6–30 ký tự, chỉ chữ cái/chữ số/dấu chấm, không chỉ bao gồm toàn chữ số).
  - Phát hiện các lỗi gõ sai tên miền phổ biến (`gmial.com`, `gamil.com`, `gmail.con`, `yaho.com`, `hotmial.com`, `outlok.com`, `fpt.edu.v`...) kèm gợi ý sửa chính xác.
  - Chặn các dịch vụ email ảo/tạm thời (`yopmail.com`, `mailinator.com`, `10minutemail.com`, `tempmail.com`...) và chặn chuỗi ký tự lặp bất thường (`22222`, `1111`, `aaaa`...) khi người dùng nhập email giả lập.
  - Xác minh trực tuyến bản ghi máy chủ nhận thư DNS MX (`type=15`) qua Google DNS-over-HTTPS (`https://dns.google/resolve`).
- **Bổ Sung API Kiểm Tra Email Trực Tuyến (`GET /api/auth/validate-email` - UC 01a)**:
  - Triển khai `ValidateEmailQuery` và `ValidateEmailHandler` kiểm tra đồng thời tính hợp lệ của email, sự tồn tại của máy chủ mail MX và trạng thái tài khoản trong CSDL `v_eval_identity.users`.
- **Nâng Cấp Nghiệp Vụ Đăng Ký (`RegisterUserCommand.cs`)**:
  - Tích hợp `EmailValidationHelper.ValidateEmailComprehensiveAsync` vào cả `RegisterUserValidator` và `RegisterUserHandler`.
  - Xử lý thông minh tài khoản chưa kích hoạt OTP (`IsActive == false`): Cho phép cập nhật lại thông tin và cấp mã OTP mới thay vì báo lỗi `409 Conflict` khóa cứng email chưa kích hoạt.
- **Triển Khai Phân Hệ Liên Kết Phụ Huynh - Học Sinh 3 Phương Thức (`ParentLinkingFeatures.cs`, `UserRepository.cs`, `UsersController.cs`)**:
  - **Phương thức 1 (Mã Tài Khoản Học Sinh `VE-XXXXXX` & Mã PIN 6 Số)**: Sinh mã định danh cố định `VE-XXXXXX` từ `StudentId` kèm mã PIN 6 số động hạn 30 phút (`PARENT_LINK_CODE`) trong bảng `v_eval_identity.OtpVerifications`; endpoint `POST /api/v1/users/parent-link/connect` cho phép Phụ huynh nhập mã kết nối tức thì vào bảng `v_eval_identity.ParentStudentRelations`.
  - **Phương thức 2 (Đường Liên Kết Mời 1-Click & Mã QR)**: Tự động bóc tách tham số `?parentLinkCode=...` hoặc `?studentCode=...` từ đường dẫn URL / QR Code để kết nối tài khoản học sinh và phụ huynh.
  - **Phương thức 3 (Gửi Yêu Cầu Theo Email/SĐT Học Sinh & Xác Nhận 2 Chiều Trên Web)**: Endpoint `POST /api/v1/users/parent-link/request` tạo yêu cầu chờ duyệt (`PARENT_REQ:{ParentId}`); học sinh xem danh sách qua `GET /api/v1/users/me/parent-link-overview` và bấm xác nhận trực tiếp trên Web qua `POST /api/v1/users/me/parent-link-requests/{requestId}/respond` (hoàn toàn không phụ thuộc máy chủ gửi thư SMTP).

---

## [04/11/2026] - Triển Khai Phân Hệ Brevo SMTP Email Service, Khóa API Thực Tế & Khắc Phục Khởi Chạy EF Core 9

- **Kích Hoạt Tài Khoản Brevo SMTP Thực Tế**:
  - Nhập khóa API và thông tin xác thực Brevo SMTP Relay (smtp-relay.brevo.com:587, STARTTLS) với tài khoản c17a0c001@smtp-brevo.com.
  - Chuẩn hóa thông tin người gửi mang thương hiệu hệ thống: V_Eval System <veval.system@gmail.com>.
  - Thực hiện kịch bản kiểm thử trực tiếp gửi email HTML chuẩn responsive qua máy chủ trung chuyển Brevo SMTP thành công 100%.
- **Khắc Phục Cảnh Báo Khởi Động EF Core 9 (PendingModelChangesWarning)**:
  - Bổ sung ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)) tại DependencyInjection.cs, bảo đảm ứng dụng khởi động an toàn, tự động di chuyển CSDL và nạp dữ liệu mồi mà không bị ngắt quãng.

- **Thiết Kế & Triển Khai Dịch Vụ Email SMTP (`BrevoSmtpEmailService.cs`)**:
  - Tích hợp bộ thư viện công nghiệp `MailKit` & `MimeKit` 4.17.0 vào tầng `Infrastructure`.
  - Khai báo giao diện trừu tượng `IEmailService` tại tầng `Application` phục vụ gửi mã OTP kích hoạt tài khoản đăng ký (UC 01) và khôi phục mật khẩu (UC 05).
  - Hiện thực `BrevoSmtpEmailService` kết nối máy chủ chuyển tiếp Brevo SMTP Relay (`smtp-relay.brevo.com:587`, STARTTLS), hỗ trợ chế độ phòng vệ Fail-Safe ghi log an toàn.
  - Thiết kế mẫu thư HTML chuyên nghiệp thương hiệu V-Eval Adaptive Learning Platform (xanh Navy/Indigo, mã OTP monospace to rõ, hạn 10 phút).
- **Đấu Nối Nghiệp Vụ CQRS**:
  - Tích hợp gọi email tự động trong `RegisterUserHandler.cs` (UC 01) và `ForgotPasswordHandler.cs` (UC 05).
  - Bổ sung cấu hình `"Email"` trong `appsettings.json`.
- **Kiểm Thử Đơn Vị**:
  - Viết Unit Tests `RegisterUserCommandHandlerTests` xác thực gọi `IEmailService`, đạt **5/5 tests PASS 100%**.
- **Bổ Sung API Xác Thực Mã OTP Quên Mật Khẩu (UC 05b - POST /api/auth/verify-reset-otp)**:
  - Triển khai VerifyResetOtpCommand, VerifyResetOtpValidator và VerifyResetOtpHandler tại tầng Application.
  - Kiểm tra tính hợp lệ và hạn dùng của mã OTP loại RESET_PASSWORD trong CSDL trước khi cho phép chuyển sang bước đổi mật khẩu mới, chặn đứng hoàn toàn việc nhập OTP sai mà vẫn được sang bước tiếp theo.
  - Đăng ký endpoint POST /api/auth/verify-reset-otp tại AuthController.cs.
  - Kiểm thử trực tiếp qua REST: Trả về HTTP 400 (Auth.InvalidOtp) khi OTP sai, trả về HTTP 200 khi OTP hợp lệ.

---

## [03/11/2026] - Nâng Cấp Luồng Đăng Ký Tài Khoản Liên Kết Cơ Sở (Campus) & Hỗ Trợ Phụ Huynh

- **Cải Tiến Lệnh Đăng Ký Người Dùng (`RegisterUserCommand.cs`)**:
  - Bổ sung trường `CampusId` vào `RegisterUserCommand` và tự động liên kết cơ sở khi tạo profile học sinh (`Student`).
  - Thiết lập giá trị mặc định an toàn sang Cơ sở 1 - ĐHQG-HCM Thủ Đức khi request không truyền mã cơ sở, triệt tiêu lỗi ràng buộc toàn vẹn khóa ngoại `FK_Students_Campuses_campus_id`.
  - Tự động sinh hồ sơ phụ huynh (`Parents`) khi tài khoản đăng ký với vai trò `PARENT`.
- **Kiểm Thử Toàn Trình**:
  - Dịch vụ lắng nghe tại cổng `5155` (HTTP) và `5156` (gRPC), liên thông mượt mà qua API Gateway `5212`.
  - Kiểm thử thành công các API: `/api/v1/campuses` (HTTP 200), `/api/auth/register` (HTTP 200), `/api/auth/verify-account` (HTTP 200) và `/api/auth/login` (HTTP 200).

---

## [02/11/2026] - Thiết Lập Kiến Trúc Test Nền Tảng Cho IDENTITY SERVICE

- **Chuẩn Hóa Hạ Tầng Dự Án Kiểm Thử (V-Eval-Identity_Service.Tests)**:
  - Nâng cấp bộ thư viện kiểm thử: `xunit` (2.9.2), `Moq` (4.20.72), `FluentAssertions` (7.2.0), `coverlet.collector` (6.0.2).
  - Tích hợp Global Usings giúp code test ngắn gọn, sạch sẽ, không cần lặp lại namespace kiểm thử.
  - Cấu hình công cụ ReportGenerator v5.4.18 qua `.config/dotnet-tools.json` để phục vụ xuất báo cáo Code Coverage dạng HTML.
  - Phân vùng cấu trúc thư mục kiểm thử chuẩn hóa: `Commands/`, `Queries/`, `Validators/`, `Common/`.
- **Bảo Vệ Kho Lưu Trữ & Script Vận Hành**:
  - Bổ sung `TestResults/` và `*.cobertura.xml` vào `.gitignore` của dịch vụ.
  - Tạo script `Scripts/test.bat` hỗ trợ chạy test nhanh và xem báo cáo coverage trực tiếp tại máy phát triển.

## [20/10/2026] - Triển Khai gRPC Endpoints Quan Hệ Phụ Huynh - Học Sinh (Parent-Student Relations) Phục Vụ Core Flow 5
- **Triển Khai Giao Thức gRPC `GetParentStudents` & `LinkParentStudent` (`IdentityGrpcService.cs`)**:
  - `GetParentStudents`: Nhận `parent_id`, truy vấn bảng `ParentStudentRelations`, nạp hồ sơ mở rộng từ `Students` và thông tin người dùng từ `Users` (Họ tên, Email, CampusId) để trả về cho phân hệ giám sát phụ huynh (Parent Companion Portal).
  - `LinkParentStudent`: Nhận `parent_id` và `student_email`, xác minh sự tồn tại của học sinh, tự động khởi tạo profile phụ huynh nếu chưa có, và thiết lập quan hệ phụ huynh - học sinh vào bảng `ParentStudentRelations`.
- **Kiểm Thử Biên Dịch**:
  - `dotnet build` đạt 100% thành công (0 warning, 0 error).

---

## [07/10/2026] - Chuẩn Hóa Cấu Hình Môi Trường & Đồng Bộ Khung Schema Centralized Config
- **Chuẩn Hóa File Cấu Hình Mẫu (`appsettings.example.json`)**:
  - Cập nhật đúng cấu trúc `Jwt` (SecretKey, Issuer, Audience, ExpiryMinutes) đồng bộ với `appsettings.json`.
  - Bổ sung cấu hình `Kestrel` hỗ trợ giao thức Http1AndHttp2 cho gRPC port `:5156`.
  - Sử dụng placeholder an toàn cho chuỗi kết nối PostgreSQL và JWT Secret Key.
- **Tương Thích Cơ Chế Quản Lý Cấu Hình Tập Trung**:
  - Sẵn sàng đồng bộ tự động thông qua công cụ `sync_config.bat` của System-Repo.
- **Kiểm Thử Biên Dịch**:
  - `dotnet build` đạt 100% thành công (0 warning, 0 error).

---

## [04/10/2026] - Triển Khai API Cấp Phát Tài Khoản Nội Bộ Đa Vai Trò (IAM Provisioning), Quản Lý Người Dùng & Seed Tài Khoản Admin
- **Triển Khai API Cấp Phát Tài Khoản Trực Tiếp (`POST /api/v1/users/provision`)**:
  - Cho phép Quản trị viên (`ADMINISTRATOR`) và Quản lý cơ sở (`ACADEMIC_MANAGER`) cấp phát trực tiếp tài khoản cho bất kỳ vai trò nào trong hệ thống: `TEACHER`, `ACADEMIC_MANAGER`, `ACADEMIC_DIRECTOR`, `ADMINISTRATOR`, `PARENT`, `STUDENT`.
  - Cơ chế kích hoạt tức thì (`IsActive = true`), không yêu cầu xác thực OTP email qua luồng công khai.
  - Tự động liên kết vai trò vào bảng `UserRoles` và tạo bản ghi hồ sơ chuyên môn tương ứng trong các bảng `profile.*` (`Teachers`, `AcademicManagers`, `AcademicDirectors`, `Administrators`, `Students`, `Parents`).
  - Hashing mật khẩu khởi tạo an toàn bằng BCrypt (Work Factor 11).
- **Triển Khai API Quản Lý & Tra Cứu Tài Khoản (`GET /api/v1/users`, `PATCH /api/v1/users/{id}/toggle-status`)**:
  - `GetUsersQuery`: Hỗ trợ phân trang, tìm kiếm tức thì theo Họ tên / Email / SĐT, lọc theo vai trò (`role`) và cơ sở (`campusId`), kèm nạp chi tiết thông tin hồ sơ chuyên môn và cơ sở công tác.
  - `ToggleUserStatusCommand`: Cho phép quản trị viên khóa / mở khóa trạng thái hoạt động của tài khoản nhanh chóng.
- **Khởi Tạo Trực Tiếp Tài Khoản Admin Trong CSDL PostgreSQL (`v_eval_identity`)**:
  - Seed tài khoản Quản trị viên `admin` / `1234` (`admin@veval.edu.vn`) băm BCrypt, gán quyền `ADMINISTRATOR`.
  - Hỗ trợ đăng nhập linh hoạt bằng cả Username `admin` và Email qua Gateway `:5212`.
- **Kiểm Thử Biên Dịch & Vận Hành Thực Tế**:
  - `dotnet build` giải pháp `V-Eval-Identity_Service.sln` đạt 100% thành công (0 warning, 0 error).
  - Kiểm thử trực tiếp qua Gateway `:5212`:
    + `GET /api/v1/users`: Trả về HTTP 200 OK kèm danh sách người dùng.
    + `POST /api/v1/users/provision`: Cấp phát thành công Giảng viên Toán `thaynam.toan@veval.edu.vn`.
    + `POST /api/auth/login`: Tài khoản vừa cấp phát đăng nhập thành công nhận JWT token hợp lệ.
    + `PATCH /api/v1/users/{id}/toggle-status`: Khóa và mở khóa trạng thái tài khoản thành công.

---

## [22/09/2026] - Bổ Sung CampusName Vào gRPC GetStudentProfileSummary
- **Cập Nhật `identity.proto` & Server `IdentityGrpcService`**:
  - Bổ sung trường `campus_name` vào message `GetStudentSummaryResponse`.
  - Truy vấn tên cơ sở đào tạo từ bảng `iam.campuses` và trả về cho `Practice_Service`.
- **Kiểm Thử**: Biên dịch Solution sạch 100% (**0 Error, 0 Warning**), khởi chạy dịch vụ cổng 5155/5156.

---

## [20/09/2026] - Triển Khai Thực Thể Campus, 6 Actor Profiles, API Chọn Cơ Sở & Server gRPC Identity
- **Hoàn Thiện Thực Thể IAM & Hồ Sơ 6 Actor Profile (`Domain Layer`)**:
  - Thực thể IAM: [`Campus.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Iam/Campus.cs) (`campus_id`, `code`, `name`, `address`, `phone`, `is_active`, `created_at`).
  - Thực thể Profile: [`Parent.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Parent.cs), [`ParentStudentRelation.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/ParentStudentRelation.cs), [`Teacher.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Teacher.cs), [`AcademicManager.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/AcademicManager.cs), [`AcademicDirector.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/AcademicDirector.cs), [`Administrator.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/Entities/Profiles/Administrator.cs).
- **Cấu Hình EF Core Multi-Schema & Seed Data (`Infrastructure Layer`)**:
  - [`AppDbContext.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/AppDbContext.cs): Đăng ký toàn bộ `DbSet` cho `Campuses` và 6 Actor profiles.
  - [`AuthConfigurations.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Configurations/AuthConfigurations.cs): Định nghĩa Fluent API mapping chuẩn xác vào schema `iam` và `profile`, quan hệ 1-1 với `iam.users` (Cascade Delete), quan hệ `campuses`, và seed dữ liệu cho 6 vai trò hệ thống cùng 2 cơ sở mẫu (CS Thủ Đức, CS Quận 10).
  - Triển khai [`ICampusRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/ICampusRepository.cs) và [`CampusRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/CampusRepository.cs) vào DI.
- **Nâng Cấp Nghiệp Vụ Hồ Sơ & Tách Biệt Rạch Ròi Hồ Sơ Chung / Actor Profile (`Application & API Layers`)**:
  - [`CampusesController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/CampusesController.cs): Cung cấp endpoint `GET /api/v1/campuses` (phục vụ UC 10 & UC 40).
  - [`UsersController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/UsersController.cs): Chuyên biệt cho thông tin tài khoản dùng chung (IAM Account): `GET /api/v1/users/me`, `PUT /api/v1/users/me/profile`, `POST /api/v1/users/me/change-password`. DTO `UserProfileDto` loại bỏ hoàn toàn các trường gắn riêng với học sinh.
  - [`StudentsController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/StudentsController.cs): Chuyên biệt cho Actor Học sinh với `GET /api/v1/students/me/profile` và `PUT /api/v1/students/me/profile` (`TargetScore` chuẩn ĐGNL ĐHQG-HCM V-ACT 0-1200, `CampusId`, ngày thi...).
- **Triển Khai Liên Dịch Vụ gRPC (`IdentityGrpcService`)**:
  - Hợp đồng [`identity.proto`](file:///d:/Capstone/grpc/identity.proto): Định nghĩa 2 RPC `ValidateUserPermission` và `GetStudentProfileSummary`.
  - Hiện thực [`IdentityGrpcService.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs) kế thừa `IdentityGrpc.IdentityGrpcBase`.
  - Đăng ký `app.MapGrpcService<IdentityGrpcService>()` trong [`Program.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Program.cs).
- **Kiểm Thử Biên Dịch & Swagger**:
  - `dotnet build "d:\Capstone\All Services\V-Eval-Identity_Service\V-Eval-Identity_Service.sln"`: **Build succeeded (0 Error(s), 0 Warning(s))**.
  - Swagger UI tại `http://localhost:5155`: Phân nhóm rõ ràng `Users`, `Students`, `Campuses`, `Auth`.

## [18/09/2026] - Bổ Sung Trọn Bộ API Quên Mật Khẩu (OTP), Đặt Lại Mật Khẩu & Đăng Xuất (Logout)
- **Triển Khai Tính Năng Quên & Đặt Lại Mật Khẩu (Forgot / Reset Password Flow)**:
  - Tầng Domain & Common Interfaces: Cập nhật [`IRefreshTokenRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IRefreshTokenRepository.cs) bổ sung phương thức `RevokeAllByUserIdAsync(Guid userId, CancellationToken ct)`.
  - Tầng Infrastructure: Hiện thực phương thức `RevokeAllByUserIdAsync` trong [`RefreshTokenRepository.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs), thu hồi toàn bộ token còn hiệu lực của người dùng khi mật khẩu được reset.
  - Tầng Application (CQRS Commands, Handlers, Validators & DTOs):
    + [`AuthDtos.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/DTOs/AuthDtos.cs): Thêm record `ForgotPasswordResponseDto(string Email, string Message, string? OtpCode = null)`.
    + **UC 05 (Quên Mật Khẩu)**: [`ForgotPasswordCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/ForgotPassword/ForgotPasswordCommand.cs) kiểm tra tài khoản hoạt động, sinh mã OTP 6 số lưu vào `iam.otp_verifications` với `Type = "RESET_PASSWORD"`, thời hạn 10 phút.
    + **UC 06 (Đặt Lại Mật Khẩu)**: [`ResetPasswordCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/ResetPassword/ResetPasswordCommand.cs) xác thực OTP `RESET_PASSWORD` chưa sử dụng, băm mật khẩu mới bằng BCrypt, đánh dấu OTP đã dùng (`is_used = true`), đồng thời kích hoạt `RevokeAllByUserIdAsync` hủy bỏ mọi phiên đăng nhập cũ trên tất cả thiết bị.
- **Triển Khai Tính Năng Đăng Xuất (Logout Flow)**:
  + **UC 07 (Đăng Xuất)**: [`LogoutCommand.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/Features/Auth/Commands/Logout/LogoutCommand.cs) nhận `RefreshToken`, tìm trong `iam.refresh_tokens` và đánh dấu `IsRevoked = true` nhằm vô hiệu hóa khả năng cấp mới Access Token của phiên làm việc đó.
- **Tầng API Endpoints ([`AuthController.cs`](file:///d:/Capstone/All%20Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/Controllers/AuthController.cs))**:
  + Bổ sung 3 endpoints công khai:
    - `POST /api/Auth/forgot-password`: Tiếp nhận email, phát hành OTP.
    - `POST /api/Auth/reset-password`: Đổi mật khẩu mới kèm OTP.
    - `POST /api/Auth/logout`: Đăng xuất, vô hiệu hóa Refresh Token.
- **Kiểm Thử Vận Hành & Build**:
  - Chạy biên dịch toàn bộ Solution `dotnet build "d:\Capstone\All Services\V-Eval-Identity_Service"` đạt **0 Error(s), 0 Warning(s)**.

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
