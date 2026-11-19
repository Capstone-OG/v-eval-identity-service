# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [19/11/2026] - Chuẩn Hóa Phân Giải Thực Thể Động & Dữ Liệu Thực Tế Trong Identity gRPC Service

- **Chuẩn Hóa Phân Giải Dữ Liệu Học Sinh & Giáo Viên ([`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs))**:
  - Trích xuất 100% dữ liệu thực tế từ các bảng `v_eval_identity.Students`, `Users` và `Teachers`.
  - Trong `GetStudentProfileSummary` và `GetParentStudents`: Trả về đúng trường THPT (`SchoolName`) và điểm mục tiêu (`TargetScore`) thực tế từ CSDL; nếu trường dữ liệu chưa được cập nhật, trả về giá trị rỗng/mặc định số học thay vì gán tên trường cố định.
  - Trong `GetTeacherProfile`: Trả về chính xác thông tin chuyên môn (`Specialty`) từ hồ sơ giảng viên thật hoặc chuỗi rỗng khi chưa khai báo.
- **Kiểm Thử & Nghiệm Thu**:
  - `dotnet build`: Hoàn thành thành công, 0 lỗi, 0 cảnh báo.
  - `dotnet test`: Toàn bộ **5/5 unit tests Passed 100%**.
