# Nhật Ký Cập Nhật (Update Log) - Identity Service

## \[23/09/2026\] - Đồng Bộ Schema `v_eval_identity` Trong AppDbContext

- **Đồng Bộ Default Schema**: Đã cập nhật `AppDbContext.cs` thiết lập `modelBuilder.HasDefaultSchema("v_eval_identity");` đồng bộ 100% với DDL CSDL PostgreSQL Schema V2.
- **Kiểm Thử Biên Dịch**: `dotnet build` giải pháp `V-Eval-Identity_Service.sln` thành công 100% (0 Errors, 0 Warnings).