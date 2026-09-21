namespace Domain.Entities.Profiles;

public class Parent
{
    public Guid ParentId { get; set; } // PK, FK sang iam.users(user_id)
    public string? PhoneWork { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ParentStudentRelation> StudentRelations { get; set; } = new List<ParentStudentRelation>();
}
