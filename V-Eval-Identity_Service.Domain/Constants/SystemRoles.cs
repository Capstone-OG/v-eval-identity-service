namespace Domain.Constants;

public static class SystemRoles
{
    public const string Student = "STUDENT";
    public const string Parent = "PARENT";
    public const string Teacher = "TEACHER";
    public const string AcademicManager = "ACADEMIC_MANAGER";
    public const string AcademicDirector = "ACADEMIC_DIRECTOR";
    public const string Administrator = "ADMINISTRATOR";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Student,
        Parent,
        Teacher,
        AcademicManager,
        AcademicDirector,
        Administrator
    };
}
