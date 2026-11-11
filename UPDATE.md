# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [07/11/2026] - Phân Hệ Quản Lý Tài Khoản Ngừng Hoạt Động (Soft Delete / Inactive), Lọc Trạng Thái & Tích Hợp Kiểm Tra Email Đa Tầng Cấp Phát IAM

- **Phân Hệ Quản Lý Tài Khoản Ngừng Hoạt Động & Thống Kê Số Lượng Realtime ([`GetUsersQuery.cs`](./V-Eval-Identity_Service.Application/Features/Users/Queries/GetUsers/GetUsersQuery.cs), [`UserRepository.cs`](./V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/UserRepository.cs), [`UsersController.cs`](./V-Eval-Identity_Service.API/Controllers/UsersController.cs))**:
  - Bổ sung bộ lọc `bool? isActive` vào lệnh `GetUsersQuery` và endpoint `GET /api/v1/users`, hỗ trợ lọc chính xác tài khoản đang hoạt động (`isActive = true`) hoặc tài khoản đã ngừng hoạt động/ẩn (`isActive = false`).
  - Nâng cấp `PagedUsersResult` trả về đồng thời `ActiveCount` và `InactiveCount` theo thời gian thực từ bảng `v_eval_identity."Users"`.
  - Nâng cấp `ToggleUserStatusCommand` (`PATCH /api/v1/users/{id}/toggle-status`) hỗ trợ tham số `targetStatus` tùy chọn, cho phép chủ động đặt trạng thái kích hoạt hoặc ngừng hoạt động có kiểm soát.
  - Tuân thủ nghiêm ngặt nguyên tắc Soft Delete: Không bao giờ xóa cứng dữ liệu khỏi bảng `Users`, bảo toàn 100% lịch sử nộp bài thi, giao dịch học phí và liên kết phụ huynh.
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
