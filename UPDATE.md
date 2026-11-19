# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [19/11/2026] - Bổ Sung Trích Xuất Cơ Sở Phụ Trách (`CampusId`, `CampusName`) & Cung Cấp RPC Truy Vấn Tên Học Sinh Theo Batch Cho Practice Service

- **Triển Khai RPC Truy Vấn Danh Sách Học Sinh Theo Batch Qua gRPC ([`identity.proto`](./V-Eval-Identity_Service.API/Protos/identity.proto), [`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs))**:
  - Bổ sung RPC `rpc GetStudentsByIds (GetStudentsByIdsRequest) returns (GetStudentsByIdsResponse)` trong `identity.proto`.
  - Trong `IdentityGrpcService.cs`, truy vấn trực tiếp bảng `iam.users` theo danh sách `student_ids` và trả về `FullName`, `Email`, `CampusId`, cho phép Practice Service đồng bộ tên học sinh thực tế thay vì hardcode.
  - Bổ sung `string full_name = 8;` vào `GetStudentSummaryResponse`.
- **Mở Rộng DTO Hồ Sơ Người Dùng & Phản Hồi Xác Thực ([`UserProfileDto.cs`](./V-Eval-Identity_Service.Application/Features/Users/DTOs/UserProfileDto.cs), [`AuthDtos.cs`](./V-Eval-Identity_Service.Application/Features/Auth/DTOs/AuthDtos.cs))**:
  - Bổ sung trường `Guid? CampusId = null` và `string? CampusName = null` vào `UserProfileDto` (phục vụ `GET /api/v1/users/me`) và `UserDto` (phục vụ `POST /api/auth/login`, `POST /api/auth/refresh-token`).
- **Triển Khai Truy Vấn Cơ Sở Phụ Trách Trong Repository ([`IUserRepository.cs`](./V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IUserRepository.cs), [`UserRepository.cs`](./V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/UserRepository.cs))**:
  - Bổ sung phương thức `GetUserCampusInfoAsync(Guid userId, CancellationToken cancellationToken)` tự động tra cứu cơ sở trực thuộc từ các bảng profile: `iam.academic_managers`, `iam.teachers`, và `iam.students`, kèm tra cứu tên cơ sở `CampusName` từ bảng `iam.campuses`.
- **Đồng Bộ Dữ Liệu Trong Handler ([`GetCurrentUserQuery.cs`](./V-Eval-Identity_Service.Application/Features/Users/Queries/GetCurrentUser/GetCurrentUserQuery.cs), [`LoginCommand.cs`](./V-Eval-Identity_Service.Application/Features/Auth/Commands/Login/LoginCommand.cs), [`RefreshTokenCommand.cs`](./V-Eval-Identity_Service.Application/Features/Auth/Commands/RefreshToken/RefreshTokenCommand.cs))**:
  - Nạp đầy đủ `CampusId` và `CampusName` ngay khi người dùng đăng nhập hoặc tải lại thông tin cá nhân `users/me`.
- **Kiểm Thử & Nghiệm Thu**:
  - `dotnet build`: Biên dịch thành công 100% trên toàn bộ solution `V-Eval-Identity_Service` với .NET 9 SDK (`0 Warning(s), 0 Error(s)`).
