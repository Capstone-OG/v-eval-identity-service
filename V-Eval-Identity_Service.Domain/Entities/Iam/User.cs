namespace Domain.Entities.Iam;

public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
