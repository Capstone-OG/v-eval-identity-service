# Nhật Ký Cập Nhật (Update Log) - Identity Service

## [14/09/2026] - Chuẩn Hóa Cấu Hình Production & Git Security
- **Khởi Tạo `appsettings.example.json`**:
  - Tạo file cấu hình mẫu chứa `ConnectionStrings` (PostgreSQL Supabase) và `JwtSettings` (SecretKey, Issuer, Audience, ExpiryInMinutes, RefreshTokenExpiryInDays).
  - Điền các label mẫu (`YOUR_POSTGRES_HOST`, `YOUR_POSTGRES_USER`, `YOUR_POSTGRES_PASSWORD`).
- **Bảo Mật Git Security**:
  - Cập nhật `.gitignore` ẩn tất cả file `appsettings.json` chứa mật khẩu cá nhân.
  - Cấu hình định tuyến qua Gateway tại `/api/identity/{**catch-all}` (Cổng `:5001`).

## [08/09/2026] - Sửa Lỗi CI & Tối Ưu Hóa GitHub Actions
- Fix CI build errors in GitHub Workflow `ci.yml`.
