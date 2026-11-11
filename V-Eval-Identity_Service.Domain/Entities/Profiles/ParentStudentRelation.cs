namespace Domain.Entities.Profiles;

public class ParentStudentRelation
{
    public Guid RelationId { get; set; } = Guid.NewGuid();
    public Guid ParentId { get; set; }
    public Parent Parent { get; set; } = null!;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
