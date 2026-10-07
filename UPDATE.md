# Nhật Ký Cập Nhật (Update Log) - Identity Service

## [07/10/2026] - Chuẩn Hóa Cấu Hình Môi Trường & Đồng Bộ Khung Schema Centralized Config

- **Chuẩn Hóa File Cấu Hình Mẫu (`appsettings.example.json`)**:
  - Cập nhật đúng cấu trúc `Jwt` (SecretKey, Issuer, Audience, ExpiryMinutes) đồng bộ với `appsettings.json`.
  - Bổ sung cấu hình `Kestrel` hỗ trợ giao thức Http1AndHttp2 cho gRPC port `:5156`.
  - Sử dụng placeholder an toàn cho chuỗi kết nối PostgreSQL và JWT Secret Key.
- **Tương Thích Cơ Chế Quản Lý Cấu Hình Tập Trung**:
  - Sẵn sàng đồng bộ tự động thông qua công cụ `sync_config.bat` của System-Repo.
- **Kiểm Thử Biên Dịch**:
  - `dotnet build` đạt 100% thành công (0 warning, 0 error).