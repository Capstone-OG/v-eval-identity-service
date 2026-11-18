# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - IDENTITY SERVICE

## [18/11/2026] - Bổ Sung gRPC RPC `GetTeacherProfile` & Mở Rộng `GetParentStudents` Cho Identity Service

- **Mở Rộng Hợp Đồng gRPC Protocol Buffers ([`identity.proto`](./V-Eval-Identity_Service.API/Protos/identity.proto))**:
  - Bổ sung RPC mới: `rpc GetTeacherProfile (GetTeacherProfileRequest) returns (GetTeacherProfileResponse);`.
  - Khai báo message `GetTeacherProfileRequest` (nhận `teacher_id`) và `GetTeacherProfileResponse` (trả về `exists`, `full_name`, `phone`, `email`, `specialty`).
  - Mở rộng `StudentSummaryItem` trong phản hồi `GetParentStudentsResponse` bổ sung `school_name = 5;` và `target_score = 6;`.
  - Mở rộng `GetStudentSummaryResponse` bổ sung `school_name = 8;`.
- **Hiện Thực Hóa gRPC Server ([`IdentityGrpcService.cs`](./V-Eval-Identity_Service.API/Services/IdentityGrpcService.cs))**:
  - Triển khai phương thức `GetTeacherProfile`: truy vấn `_context.Users` và `_context.Teachers` theo `teacher_id`, trả về thông tin danh tính, số điện thoại liên hệ và chuyên môn của giảng viên.
  - Cập nhật phương thức `GetParentStudents`: điền `student.SchoolName` và `student.TargetScore` từ bảng `profile.students`.
  - Cập nhật phương thức `GetStudentProfileSummary`: điền `SchoolName` vào phản hồi hồ sơ học sinh.
- **Kiểm Thử & Nghiệm Thu**:
  - Biên dịch .NET 9 SDK đạt chuẩn sạch (0 Warning, 0 Error).
  - Chạy `dotnet test`: Toàn bộ **5/5 tests Passed 100%**.
