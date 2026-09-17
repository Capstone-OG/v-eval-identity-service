# Nhật ký & Kế hoạch Phát triển Identity Service (Daily Process and Planning)

Tài liệu này dùng để theo dõi tiến độ phát triển thực tế hàng ngày, các cột mốc tính năng (Milestones) và kế hoạch tương lai của phân hệ **V-Eval-Identity_Service**.

---

## 📅 Cập nhật ngày 18/09/2026

### 🎯 Mục tiêu hiện tại (Milestone 3 - Authentication & User Profile Core Module)
Triển khai toàn bộ tính năng Xác thực & Quản lý người dùng (Authentication & User Profile) theo chuẩn Clean Architecture 4 tầng, CQRS (MediatR), Result Pattern, và Multi-Schema PostgreSQL (`iam` & `profile`).

### 📋 Danh sách Task & Trạng thái

| Tên Task | Trạng thái | Ghi chú |
| :--- | :---: | :--- |
| **Thiết kế Domain Layer** | 🟢 Hoàn thành | Định nghĩa Entity `User`, `Role`, `UserRole`, `RefreshToken`, `OtpVerification` (`iam`) và `Student` (`profile`), hằng số `SystemRoles`. |
| **Triển khai Application Layer** | 🟢 Hoàn thành | Xây dựng Result Pattern (`Result<T>`, `Error`), `ValidationBehavior` tích hợp FluentValidation, Interfaces và toàn bộ Command/Query Handlers. |
| **Triển khai Infrastructure Layer** | 🟢 Hoàn thành | Cấu hình EF Core multi-schema `iam` & `profile`, triển khai **Cả Hai: Repository Pattern & Unit of Work**, dịch vụ BCrypt, JWT Provider, OtpService. |
| **Tự động hóa Migration & Seed Data** | 🟢 Hoàn thành | Áp dụng EF Core Migration tạo bảng trên PostgreSQL Supabase, tự động seed 6 vai trò hệ thống (`STUDENT`, `PARENT`, `TEACHER`, `ACADEMIC_MANAGER`, `ACADEMIC_DIRECTOR`, `ADMINISTRATOR`). |
| **Triển khai API Layer & Swagger UI** | 🟢 Hoàn thành | Viết `ApiControllerBase`, `AuthController`, `UsersController` (`[Authorize]`), Global Exception Middleware, Swagger UI có nút Authorize JWT Bearer. |
| **Kiểm thử tích hợp E2E tự động** | 🟢 Hoàn thành | Kiểm thử 9/9 bước luồng Register -> Chặn 403 -> Verify OTP -> Login JWT -> Get Me -> Update Profile -> Change Password -> Refresh Token đạt 100% Passed. |

---

## 📅 Cập nhật ngày 15/09/2026

### 🎯 Mục tiêu hiện tại (Milestone 2 - Docker Containerization)
Tạo Dockerfile Multi-stage cho Identity Service (Cổng `:5001`) và kết nối vào `docker-compose.yml`.

### 📋 Danh sách Task & Trạng thái

| Tên Task | Trạng thái | Ghi chú |
| :--- | :---: | :--- |
| **Tạo `Dockerfile` Multi-stage** | 🟢 Hoàn thành | Build và publish .NET 9 API image trên cổng `5001`. |
| **Tích hợp Docker Compose** | 🟢 Hoàn thành | Khai báo container `v_eval_identity_service` tham gia mạng `veval_network`. |

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
