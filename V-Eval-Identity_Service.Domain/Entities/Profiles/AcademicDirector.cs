namespace Domain.Entities.Profiles;

public class AcademicDirector
{
    public Guid DirectorId { get; set; } // PK, FK sang iam.users(user_id)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
