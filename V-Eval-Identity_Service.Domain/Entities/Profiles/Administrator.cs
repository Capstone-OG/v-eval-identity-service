namespace Domain.Entities.Profiles;

public class Administrator
{
    public Guid AdminId { get; set; } // PK, FK sang iam.users(user_id)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
