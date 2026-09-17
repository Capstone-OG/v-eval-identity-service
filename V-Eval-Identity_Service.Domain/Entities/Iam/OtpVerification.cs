namespace Domain.Entities.Iam;

public class OtpVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string OtpCode { get; set; } = null!; // Mã 6 số, ví dụ "123456"
    public string Type { get; set; } = null!;    // "REGISTER_VERIFY", "RESET_PASSWORD"
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
