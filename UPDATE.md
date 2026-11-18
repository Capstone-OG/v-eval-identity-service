# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [19/11/2026] - Trả Về Exists = False Khi Không Tìm Thấy Giáo Viên Trong CSDL (Triệt Tiêu Hoàn Toàn Mock Fallback)

- **Triệt Tiêu Giả Lập Trong gRPC Server ([`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs))**:
  - Xóa bỏ 100% việc tiêm dữ liệu `"ThS. Nguyễn Minh Triết"` khi không tìm thấy giáo viên trong CSDL.
  - Khi `user == null && teacher == null`, `GetTeacherProfile` trả về chính xác `Exists = false`.
  - Khi tìm thấy giáo viên thật trong bảng `v_eval_identity.Users` và `Teachers`, trả về đúng họ tên, SĐT, email và chuyên môn từ DB.
- **Kiểm Thử & Nghiệm Thu**:
  - `dotnet test`: Toàn bộ **5/5 unit tests Passed 100%**.
