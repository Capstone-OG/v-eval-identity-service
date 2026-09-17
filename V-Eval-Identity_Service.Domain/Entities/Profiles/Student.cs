namespace Domain.Entities.Profiles;

public class Student
{
    // Khóa chính đồng thời là Khóa ngoại 1-1 trỏ sang iam.users(user_id)
    public Guid StudentId { get; set; } 
    public Guid? CampusId { get; set; }
    
    // Các thông tin cá nhân mở rộng (Update Profile)
    public int? TargetScore { get; set; }
    public DateOnly? ExamDate { get; set; }
    public double? StudyHoursDay { get; set; }
    public string? SchoolName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
