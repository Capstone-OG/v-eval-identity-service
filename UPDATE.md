# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [25/10/2026] - Chuẩn Hóa Ràng Buộc Cơ Sở (Campus): Bắt Buộc Campus Cho Học Sinh, Bổ Sung Campus Cho Giám Đốc Đào Tạo & Chuẩn Hóa Quan Hệ Giám Hộ 1..N

- **Chuẩn Hóa Thuộc Tính & Ràng Buộc Khóa Ngoại Cơ Sở (Campuses)**:
  - Cập nhật [`Student.cs`](./V-Eval-Identity_Service.Domain/Entities/Profiles/Student.cs): Đổi `CampusId` từ `Guid?` sang `Guid` (bắt buộc `NOT NULL`).
  - Cập nhật [`AcademicDirector.cs`](./V-Eval-Identity_Service.Domain/Entities/Profiles/AcademicDirector.cs): Bổ sung `CampusId` (`Guid NOT NULL`) gắn trực tiếp Giám đốc đào tạo với cơ sở phụ trách chuyên môn.
  - Cập nhật [`AuthConfigurations.cs`](./V-Eval-Identity_Service.Infrastructure/Persistence/Configurations/AuthConfigurations.cs): Cấu hình ràng buộc `IsRequired()` và `OnDelete(DeleteBehavior.Restrict)` cho cả `StudentConfiguration` và `AcademicDirectorConfiguration`.
- **Cập Nhật Tầng Application & Tầng Giao Tiếp gRPC**:
  - Tái cấu trúc [`IUserRepository.cs`](./V-Eval-Identity_Service.Application/Common/Interfaces/Repositories/IUserRepository.cs) & [`UserRepository.cs`](./V-Eval-Identity_Service.Infrastructure/Persistence/Repositories/UserRepository.cs): Phương thức `AddAcademicDirectorProfileAsync` tiếp nhận `Guid campusId`.
  - Cập nhật [`ProvisionUserCommand.cs`](./V-Eval-Identity_Service.Application/Features/Users/Commands/ProvisionUser/ProvisionUserCommand.cs) & [`UpdateStudentProfileCommand.cs`](./V-Eval-Identity_Service.Application/Features/Students/Commands/UpdateStudentProfile/UpdateStudentProfileCommand.cs): Xử lý gán `CampusId` không nullable chuẩn xác.
  - Cập nhật [`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs): Xử lý trích xuất `student.CampusId.ToString()` không nullable trong `ValidatePermissionResponse`, `GetStudentProfileSummary` và `GetParentStudents`.
- **Đồng Bộ Kịch Bản CSDL Master [`docs/SQL/SQL.sql`](../../docs/SQL/SQL.sql)**:
  - Sửa cột `Students.campus_id` thành `uuid NOT NULL REFERENCES v_eval_identity."Campuses"("campus_id") ON DELETE RESTRICT`.
  - Bổ sung cột `AcademicDirectors.campus_id` (`uuid NOT NULL REFERENCES v_eval_identity."Campuses"("campus_id") ON DELETE RESTRICT`).
  - Thêm chỉ mục B-tree hiệu năng cao `idx_directors_campus`.
- **Biên Dịch & Kiểm Thử Solution**:
  - Chạy `dotnet build --no-restore V-Eval-Identity_Service.sln` đạt kết quả sạch 100% (**0 Error, 0 Warning**).