# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [06/11/2026] - Triển Khai Kiểm Tra Email Đa Tầng (DNS MX - UC 01a) & Hệ Thống Liên Kết Phụ Huynh - Học Sinh 3 Phương Thức (Mã Code / Link & QR / Xác Nhận 2 Chiều Trên Web - UC 41)

- **Triển Khai Hệ Thống Liên Kết Phụ Huynh - Học Sinh 3 Phương Thức (`ParentLinkingFeatures.cs`, `UserRepository.cs`, `UsersController.cs`)**:
  - **Phương thức 1 — Mã Tài Khoản Học Sinh (`VE-XXXXXX`) & Mã PIN 6 Số Động**:
    - Endpoint `GET /api/v1/users/me/parent-link-overview` cấp mã học sinh cố định `VE-XXXXXX` cùng mã PIN 6 số động hạn 30 phút (`PARENT_LINK_CODE`) trong bảng `v_eval_identity.OtpVerifications`.
    - Endpoint `POST /api/v1/users/parent-link/connect` cho phép Phụ huynh nhập mã `VE-XXXXXX`, mã PIN 6 số hoặc dán đường dẫn liên kết để thiết lập quan hệ ngay lập tức vào bảng `v_eval_identity.ParentStudentRelations`.
  - **Phương thức 2 — Đường Liên Kết Mời 1-Click & Mã QR**:
    - Hỗ trợ giải mã trực tiếp tham số `?parentLinkCode=...` hoặc `?studentCode=...` từ đường liên kết mời hoặc mã QR của học sinh.
  - **Phương thức 3 — Tìm Theo Email/SĐT Học Sinh & Xác Nhận 2 Chiều Trên Web**:
    - Endpoint `POST /api/v1/users/parent-link/request` tạo yêu cầu liên kết chờ duyệt (`PARENT_REQ:{ParentId}`) cho học sinh theo Email hoặc Số điện thoại.
    - Endpoint `POST /api/v1/users/me/parent-link-requests/{requestId}/respond` giúp Học sinh bấm **Chấp nhận / Từ chối** ngay trên Cổng Học Viên (hoàn toàn không phụ thuộc máy chủ gửi thư SMTP).
- **Triển Khai Bộ Kiểm Tra Email Đa Tầng (`EmailValidationHelper.cs`, `ValidateEmailQuery.cs`, `RegisterUserCommand.cs`)**:
  - Kiểm tra cú pháp RFC 5322, quy tắc tài khoản Gmail (6–30 ký tự), phát hiện lỗi gõ sai tên miền phổ biến (`gmial.com`, `gamil.com`...), chặn dịch vụ email ảo 10 phút, chặn chuỗi ký tự lặp bất thường (`22222`) và đối soát bản ghi máy chủ nhận thư DNS `MX` trực tuyến (`GET /api/auth/validate-email`).
