namespace Domain.Entities.Iam;

public class RefreshToken
{
    public Guid RefreshTokenId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
