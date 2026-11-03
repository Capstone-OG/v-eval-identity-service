# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [03/11/2026] - Nâng Cấp Luồng Đăng Ký Tài Khoản Liên Kết Cơ Sở (Campus) & Hỗ Trợ Phụ Huynh

- **Cải Tiến Lệnh Đăng Ký Người Dùng (`RegisterUserCommand.cs`)**:
  - Bổ sung thuộc tính `CampusId` vào `RegisterUserCommand` và tự động ánh xạ cơ sở đào tạo khi tạo hồ sơ học sinh (`Student`).
  - Thiết lập cơ chế fallback an toàn: Tự động gán cơ sở mặc định (Cơ sở 1 - ĐHQG-HCM Thủ Đức) nếu client không truyền mã cơ sở, triệt tiêu lỗi ràng buộc toàn vẹn khóa ngoại `FK_Students_Campuses_campus_id`.
  - Bổ sung nhánh xử lý tự động tạo hồ sơ phụ huynh (`Parents`) khi tài khoản đăng ký với vai trò `PARENT`.
- **Kiểm Thử & Vận Hành Trực Tuyến**:
  - Biên dịch giải pháp `V-Eval-Identity_Service` thành công 100% (0 Warnings, 0 Errors).
  - Khởi chạy dịch vụ độc lập tại cổng HTTP `5155` và gRPC `5156`, điều phối thông suốt qua API Gateway `5212`.
  - Xác thực thành công chuỗi nghiệp vụ toàn trình: Đăng ký tài khoản (`/api/auth/register`) ➔ Kích hoạt OTP 6 số (`/api/auth/verify-account`) ➔ Đăng nhập cấp JWT Token (`/api/auth/login`).
