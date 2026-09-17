namespace Domain.Entities.Iam;

public class Role
{
    public Guid RoleId { get; set; } = Guid.NewGuid();
    public string RoleName { get; set; } = null!; // VD: "STUDENT", "PARENT", "TEACHER", "ACADEMIC_MANAGER", "ACADEMIC_DIRECTOR", "ADMINISTRATOR"

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
