# Nhật Ký Cập Nhật (Update Log) - Identity Service

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