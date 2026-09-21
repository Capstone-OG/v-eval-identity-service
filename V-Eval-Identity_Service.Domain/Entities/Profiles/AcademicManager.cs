namespace Domain.Entities.Profiles;

public class AcademicManager
{
    public Guid ManagerId { get; set; } // PK, FK sang iam.users(user_id)
    public Guid CampusId { get; set; }  // FK sang iam.campuses(campus_id)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
