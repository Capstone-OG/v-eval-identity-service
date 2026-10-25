# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [25/10/2026] - Chuẩn Hóa Ràng Buộc Cơ Sở (Campus): Bắt Buộc Campus Cho Học Sinh, Xác Nhận Vai Trò Toàn Hệ Thống Của Giám Đốc Đào Tạo & Chuẩn Hóa Quan Hệ Giám Hộ 1..N

- **Chuẩn Hóa Thuộc Tính & Ràng Buộc Khóa Ngoại Cơ Sở (Campuses)**:
  - Cập nhật [`Student.cs`](./V-Eval-Identity_Service.Domain/Entities/Profiles/Student.cs): Đổi `CampusId` từ `Guid?` sang `Guid` (bắt buộc `NOT NULL REFERENCES Campuses(campus_id) ON DELETE RESTRICT`).
  - Xác nhận hồ sơ [`AcademicDirector.cs`](./V-Eval-Identity_Service.Domain/Entities/Profiles/AcademicDirector.cs): Giữ nguyên vị trí Giám đốc Đào tạo cấp cao toàn hệ thống (HQ / Head of Academics), phụ trách duyệt khung năng lực, đề thi chuẩn và kiểm duyệt dạng bài mới do AI đề xuất chung cho toàn quốc mà không gắn vào một Campus cục bộ.
  - Cập nhật [`AuthConfigurations.cs`](./V-Eval-Identity_Service.Infrastructure/Persistence/Configurations/AuthConfigurations.cs): Cấu hình ràng buộc `CampusId` của `StudentConfiguration` là `IsRequired()` và `OnDelete(DeleteBehavior.Restrict)`.
- **Cập Nhật Tầng Application & Tầng Giao Tiếp gRPC**:
  - Cập nhật [`ProvisionUserCommand.cs`](./V-Eval-Identity_Service.Application/Features/Users/Commands/ProvisionUser/ProvisionUserCommand.cs) & [`UpdateStudentProfileCommand.cs`](./V-Eval-Identity_Service.Application/Features/Students/Commands/UpdateStudentProfile/UpdateStudentProfileCommand.cs): Xử lý gán `CampusId` không nullable chuẩn xác cho học sinh.
  - Cập nhật [`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs): Xử lý trích xuất `student.CampusId.ToString()` không nullable trong `ValidatePermissionResponse`, `GetStudentProfileSummary` và `GetParentStudents`.
- **Đồng Bộ Kịch Bản CSDL Master [`docs/SQL/SQL.sql`](../../docs/SQL/SQL.sql)**:
  - Sửa cột `Students.campus_id` thành `uuid NOT NULL REFERENCES v_eval_identity."Campuses"("campus_id") ON DELETE RESTRICT`.
  - Giữ nguyên `AcademicDirectors` kế thừa `1:1` từ `Users` không phụ thuộc `campus_id`.
- **Biên Dịch & Kiểm Thử Solution**:
  - Chạy `dotnet build --no-restore V-Eval-Identity_Service.sln` đạt kết quả sạch 100% (**0 Error, 0 Warning**).