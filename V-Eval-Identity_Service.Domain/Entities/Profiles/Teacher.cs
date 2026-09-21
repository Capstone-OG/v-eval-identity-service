namespace Domain.Entities.Profiles;

public class Teacher
{
    public Guid TeacherId { get; set; } // PK, FK sang iam.users(user_id)
    public Guid CampusId { get; set; }  // FK sang iam.campuses(campus_id)
    public string? Specialty { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
