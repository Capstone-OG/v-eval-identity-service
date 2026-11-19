# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [19/11/2026] - Chuẩn Hóa Phân Giải Thực Thể Động, Trích Xuất Cơ Sở Phụ Trách & Cung Cấp RPC Truy Vấn Tên Học Sinh

- **Chuẩn Hóa Phân Giải Dữ Liệu Học Sinh & Giáo Viên ([`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs))**:
  - Trích xuất 100% dữ liệu thực tế từ các bảng `v_eval_identity.Students`, `Users` và `Teachers`.
  - Trong `GetStudentProfileSummary` và `GetParentStudents`: Trả về đúng trường THPT (`SchoolName`) và điểm mục tiêu (`TargetScore`) thực tế từ CSDL.
  - Trong `GetTeacherProfile`: Trả về chính xác thông tin chuyên môn (`Specialty`) từ hồ sơ giảng viên thật hoặc chuỗi rỗng khi chưa khai báo.
- **Triển Khai RPC Truy Vấn Danh Sách Học Sinh Theo Batch Qua gRPC ([`identity.proto`](./V-Eval-Identity_Service.API/Protos/identity.proto), [`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs))**:
  - Bổ sung RPC `rpc GetStudentsByIds (GetStudentsByIdsRequest) returns (GetStudentsByIdsResponse)` trong `identity.proto`.
  - Trong `IdentityGrpcService.cs`, truy vấn trực tiếp bảng `iam.users` theo danh sách `student_ids` và trả về `FullName`, `Email`, `CampusId`, cho phép Practice Service đồng bộ tên học sinh thực tế.
  - Bổ sung `string full_name = 9;` vào `GetStudentSummaryResponse`.
- **Mở Rộng DTO Hồ Sơ Người Dùng & Phản Hồi Xác Thực ([`UserProfileDto.cs`](./V-Eval-Identity_Service.Application/Features/Users/DTOs/UserProfileDto.cs), [`AuthDtos.cs`](./V-Eval-Identity_Service.Application/Features/Auth/DTOs/AuthDtos.cs))**:
  - Bổ sung trường `Guid? CampusId = null` và `string? CampusName = null` vào `UserProfileDto` (phục vụ `GET /api/v1/users/me`) và `UserDto` (phục vụ `POST /api/auth/login`, `POST /api/auth/refresh-token`).
- **Triển Khai Truy Vấn Cơ Sở Phụ Trách Trong Repository ([`IUserRepository.cs`](./V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IUserRepository.cs), [`UserRepository.cs`](./V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/UserRepository.cs))**:
  - Bổ sung phương thức `GetUserCampusInfoAsync(Guid userId, CancellationToken cancellationToken)` tự động tra cứu cơ sở trực thuộc từ các bảng profile: `iam.academic_managers`, `iam.teachers`, và `iam.students`, kèm tra cứu tên cơ sở `CampusName` từ bảng `iam.campuses`.
- **Kiểm Thử & Nghiệm Thu**:
  - `dotnet build`: Hoàn thành thành công, 0 lỗi, 0 cảnh báo.
  - `dotnet test`: Toàn bộ **5/5 unit tests Passed 100%**.
