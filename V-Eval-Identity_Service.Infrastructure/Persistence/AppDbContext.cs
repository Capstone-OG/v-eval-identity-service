using Domain.Entities.Iam;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // IAM Entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
    public DbSet<Campus> Campuses => Set<Campus>();

    // Profile Entities
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<ParentStudentRelation> ParentStudentRelations => Set<ParentStudentRelation>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<AcademicManager> AcademicManagers => Set<AcademicManager>();
    public DbSet<AcademicDirector> AcademicDirectors => Set<AcademicDirector>();
    public DbSet<Administrator> Administrators => Set<Administrator>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
