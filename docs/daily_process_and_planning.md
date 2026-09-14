# Nhật ký & Kế hoạch Phát triển Identity Service (Daily Process and Planning)

Tài liệu này dùng để theo dõi tiến độ phát triển thực tế hàng ngày, các cột mốc tính năng (Milestones) và kế hoạch tương lai của phân hệ **V-Eval-Identity_Service**.

---

## 📅 Cập nhật ngày 14/09/2026

### 🎯 Mục tiêu hiện tại (Milestone 1 - Security & Configuration)
Khởi tạo cấu hình mẫu Production (`appsettings.example.json`), ẩn các file cấu hình bí mật local qua `.gitignore`, và đồng bộ định tuyến Gateway API.

### 📋 Danh sách Task & Trạng thái

| Tên Task | Trạng thái | Ghi chú |
| :--- | :---: | :--- |
| **Khởi Tạo `appsettings.example.json`** | 🟢 Hoàn thành | Tạo file mẫu chứa đầy đủ `ConnectionStrings` và `JwtSettings` với label mẫu. |
| **Bảo Mật Git Security** | 🟢 Hoàn thành | Cập nhật `.gitignore` ẩn tất cả file `appsettings.json` chứa thông tin nhạy cảm. |
| **Định Tuyến Gateway YARP** | 🟢 Hoàn thành | Định tuyến `/api/identity/{**catch-all}` tại Gateway V-Eval trỏ về cổng `:5001`. |

---

## 📅 Kế hoạch Tiếp theo (Upcoming Roadmap)

### 🎯 Milestone 2: Đăng ký, Đăng nhập & Sinh JWT Token
- [ ] Minimal API / CQRS Controller cho `POST /api/identity/register` & `POST /api/identity/login`.
- [ ] Phát hành JWT Bearer Token chứa `sub` (UserId), `role`, `email`.
- [ ] Tích hợp Refresh Token và thu hồi Token (Token Revocation).
