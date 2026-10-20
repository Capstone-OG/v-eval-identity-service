# Nhật Ký Cập Nhật (Update Log) - Identity Service

## [20/10/2026] - Triển Khai gRPC Endpoints Quan Hệ Phụ Huynh - Học Sinh (Parent-Student Relations) Phục Vụ Core Flow 5

- **Triển Khai Giao Thức gRPC `GetParentStudents` & `LinkParentStudent` (`IdentityGrpcService.cs`)**:
  - `GetParentStudents`: Nhận `parent_id`, truy vấn bảng `ParentStudentRelations`, nạp hồ sơ mở rộng từ `Students` và thông tin người dùng từ `Users` (Họ tên, Email, CampusId) để trả về cho phân hệ giám sát phụ huynh (Parent Companion Portal).
  - `LinkParentStudent`: Nhận `parent_id` và `student_email`, xác minh sự tồn tại của học sinh, tự động khởi tạo profile phụ huynh nếu chưa có, và thiết lập quan hệ phụ huynh - học sinh vào bảng `ParentStudentRelations`.
- **Kiểm Thử Biên Dịch**:
  - `dotnet build` đạt 100% thành công (0 warning, 0 error).