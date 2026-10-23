# Nhật Ký Cập Nhật (Update Log) - Identity Service

## [23/10/2026] - Đồng Bộ CSDL Định Danh, RBAC & Hồ Sơ 6 Actors Vào Master SQL.sql Và Capstone.drawio

- **Đồng Bộ CSDL Phân Hệ Identity (`v_eval_identity`)**:
  - Hợp nhất toàn bộ 13 bảng thực thể vào `docs/SQL/SQL.sql`: `Campuses`, `Roles`, `Users`, `UserRoles`, `RefreshTokens`, `OtpVerifications`, `Students`, `Parents`, `ParentStudentRelations`, `Teachers`, `AcademicManagers`, `AcademicDirectors`, `Administrators`.
  - Chuẩn hóa đầy đủ các trường bảo mật phiên (`RefreshTokens`), mã xác thực (`OtpVerifications`), và kế thừa 1:1 Actor Profiles từ `Users`.
- **Đồng Bộ ERD Trang 1 Trong `docs/SQL/Capstone.drawio`**:
  - Phân bổ vào 2 package: `1. Định danh & RBAC` (6 bảng) và `2. Hồ sơ Actor` (7 bảng).