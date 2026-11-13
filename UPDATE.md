# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [11/11/2026] - Chuyển Nhánh Main, Chuẩn Hóa Cấu Hình Kestrel ListenAnyIP & Đóng Gói Docker Container Cho Hệ Thống V-Eval

- **Cấu Hình Kestrel Lắng Nghe Đa Giao Diện Mạng ([`Program.cs`](./V-Eval-Identity_Service.API/Program.cs))**:
  - Chuyển đổi từ cấu hình `options.ListenLocalhost(...)` sang `options.ListenAnyIP(...)`:
    - Cổng `5155`: Lắng nghe HTTP/1 phục vụ toàn bộ REST API điều phối người dùng, xác thực IAM, quản lý cơ sở và các endpoint chuyển trạng thái tài khoản.
    - Cổng `5156`: Lắng nghe HTTP/2 phục vụ dịch vụ gRPC Server cho các microservice nội bộ (`IdentityGrpcService`).
  - Cho phép Kestrel chấp nhận các kết nối liên container từ `veval_network` trong Docker Compose và kết nối từ bên ngoài máy chủ.

- **Chuẩn Hóa Đóng Gói Docker Production ([`Dockerfile`](./Dockerfile), [`docker-compose.yml`](../../docker-compose.yml))**:
  - Cập nhật chỉ thị `EXPOSE 5155 5156` và biến môi trường `ENV ASPNETCORE_URLS=http://+:5155;http://+:5156` trong Dockerfile.
  - Cập nhật cấu hình Docker Compose ánh xạ hai cổng `5155:5155` và `5156:5156`, đồng bộ URL gRPC với Practice Service và URL REST với Gateway cluster.

- **Kiểm Thử & Nghiệm Thu Trực Tuyến**:
  - `dotnet build`: Biên dịch thành công 100% với .NET 9 SDK (0 Error, 0 Warning).
  - Docker Image `capstone-v-eval-identity-service:latest` build thành công trong 117.9 giây.
  - Container `v_eval_identity_service` khởi chạy ở trạng thái `Up`, phục vụ dữ liệu trực tiếp:
    - Endpoint `https://api.pickic.io.vn/api/v1/campuses` phản hồi HTTP 200 kèm danh sách 2 cơ sở đào tạo chuẩn.
