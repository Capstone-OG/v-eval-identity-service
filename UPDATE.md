# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [07/11/2026] - Tích Hợp Kiểm Tra Email Đa Tầng (DNS MX & Chống Typo mail.com) Vào Cấp Phát Tài Khoản (IAM Provisioning) & Xử Lý Triệt Để Lỗi Null CampusId

- **Tích Hợp Kiểm Tra Email Đa Tầng Vào Cấp Phát Tài Khoản ([`ProvisionUserCommand.cs`](./V-Eval-Identity_Service.Application/Features/Users/Commands/ProvisionUser/ProvisionUserCommand.cs))**:
  - Tích hợp `EmailValidationHelper.ValidateEmailComprehensiveAsync` vào lệnh cấp phát tài khoản trực tiếp `ProvisionUserCommand`.
  - Bổ sung phát hiện lỗi gõ nhầm tên miền `mail.com` thành `gmail.com` trong [`EmailValidationHelper.cs`](./V-Eval-Identity_Service.Application/Common/Validators/EmailValidationHelper.cs) (`CommonDomainTypos`).
  - Chặn đứng các trường hợp nhập sai email, tên miền không có bản ghi DNS MX, dịch vụ email rác/ảo 10 phút và chuỗi ký tự lặp giả mạo (`22222`).
- **Khắc Phục Lỗi Null `CampusId` Khi Tải Danh Sách Người Dùng ([`Student.cs`](./V-Eval-Identity_Service.Domain/Entities/Profiles/Student.cs), [`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs))**:
  - Chuyển trường `CampusId` trong thực thể `Student` sang dạng nullable (`Guid?`) cho phép EF Core ánh xạ an toàn các bản ghi học sinh cũ chưa gán cơ sở.
  - Cập nhật logic xử lý an toàn trong `IdentityGrpcService.cs`, loại bỏ hoàn toàn ngoại lệ `InvalidCastException: Column 'campus_id' is null` gây lỗi HTTP 500 khi gọi `GET /api/v1/users`.
  - Cập nhật 5 bản ghi học sinh thiếu cơ sở trong CSDL PostgreSQL sang cơ sở mặc định `CS_THU_DUC` (`3fa85f64-5717-4562-b3fc-2c963f66afa6`).
- **Kiểm Thử & Vận Hành**:
  - Biên dịch .NET 9 SDK đạt chuẩn 100% sạch (0 Warning, 0 Error).
  - Toàn bộ 5/5 kiểm thử xUnit đạt Passed 100%.
  - Endpoint `POST /api/v1/users/provision` chặn thành công email sai chính tả `@mail.com` và gợi ý `@gmail.com`.
