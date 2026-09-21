using Domain.Entities.Iam;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CampusConfiguration : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        builder.ToTable("campuses", "iam");
        builder.HasKey(c => c.CampusId);
        builder.Property(c => c.CampusId).HasColumnName("campus_id");
        builder.Property(c => c.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.HasIndex(c => c.Code).IsUnique();
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(c => c.Address).HasColumnName("address").HasMaxLength(255).IsRequired();
        builder.Property(c => c.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(c => c.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");

        // Seed dữ liệu cơ sở mẫu phục vụ UC 10 & UC 40
        builder.HasData(
            new Campus
            {
                CampusId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Code = "CS_THU_DUC",
                Name = "Cơ sở 1 - Khu đô thị ĐHQG-HCM, TP. Thủ Đức",
                Address = "Khu phố 6, Phường Linh Trung, TP. Thủ Đức, TP.HCM",
                Phone = "02837242160",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Campus
            {
                CampusId = Guid.Parse("4ba85f64-5717-4562-b3fc-2c963f66afa7"),
                Code = "CS_QUAN_10",
                Name = "Cơ sở 2 - Quận 10, TP.HCM",
                Address = "268 Lý Thường Kiệt, Phường 14, Quận 10, TP.HCM",
                Phone = "02838651670",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles", "iam");
        builder.HasKey(r => r.RoleId);
        builder.Property(r => r.RoleId).HasColumnName("role_id");
        builder.Property(r => r.RoleName).HasColumnName("role_name").HasMaxLength(50).IsRequired();
        builder.HasIndex(r => r.RoleName).IsUnique();
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "iam");
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId).HasColumnName("user_id");
        
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(u => u.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(u => u.AvatarUrl).HasColumnName("avatar_url");
        builder.Property(u => u.IsActive).HasColumnName("is_active").HasDefaultValue(false);
        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles", "iam");
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
        builder.Property(ur => ur.UserId).HasColumnName("user_id");
        builder.Property(ur => ur.RoleId).HasColumnName("role_id");

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens", "iam");
        builder.HasKey(rt => rt.RefreshTokenId);
        builder.Property(rt => rt.RefreshTokenId).HasColumnName("refresh_token_id");
        builder.Property(rt => rt.UserId).HasColumnName("user_id");
        builder.Property(rt => rt.Token).HasColumnName("token").HasMaxLength(500).IsRequired();
        builder.Property(rt => rt.ExpiresAt).HasColumnName("expires_at");
        builder.Property(rt => rt.IsRevoked).HasColumnName("is_revoked");
        builder.Property(rt => rt.CreatedAt).HasColumnName("created_at");

        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OtpVerificationConfiguration : IEntityTypeConfiguration<OtpVerification>
{
    public void Configure(EntityTypeBuilder<OtpVerification> builder)
    {
        builder.ToTable("otp_verifications", "iam");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("id");
        builder.Property(o => o.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        builder.Property(o => o.OtpCode).HasColumnName("otp_code").HasMaxLength(10).IsRequired();
        builder.Property(o => o.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
        builder.Property(o => o.ExpiresAt).HasColumnName("expires_at");
        builder.Property(o => o.IsUsed).HasColumnName("is_used");
        builder.Property(o => o.CreatedAt).HasColumnName("created_at");
    }
}

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students", "profile");
        builder.HasKey(s => s.StudentId);
        builder.Property(s => s.StudentId).HasColumnName("student_id");
        builder.Property(s => s.CampusId).HasColumnName("campus_id");
        builder.Property(s => s.TargetScore).HasColumnName("target_score");
        builder.Property(s => s.ExamDate).HasColumnName("exam_date");
        builder.Property(s => s.StudyHoursDay).HasColumnName("study_hours_day");
        builder.Property(s => s.SchoolName).HasColumnName("school_name").HasMaxLength(255);
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");

        // Quan hệ 1-1 với iam.users
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Student>(s => s.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Quan hệ với iam.campuses
        builder.HasOne<Campus>()
            .WithMany()
            .HasForeignKey(s => s.CampusId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ParentConfiguration : IEntityTypeConfiguration<Parent>
{
    public void Configure(EntityTypeBuilder<Parent> builder)
    {
        builder.ToTable("parents", "profile");
        builder.HasKey(p => p.ParentId);
        builder.Property(p => p.ParentId).HasColumnName("parent_id");
        builder.Property(p => p.PhoneWork).HasColumnName("phone_work").HasMaxLength(20);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Parent>(p => p.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ParentStudentRelationConfiguration : IEntityTypeConfiguration<ParentStudentRelation>
{
    public void Configure(EntityTypeBuilder<ParentStudentRelation> builder)
    {
        builder.ToTable("parent_student_relations", "profile");
        builder.HasKey(r => r.RelationId);
        builder.Property(r => r.RelationId).HasColumnName("relation_id");
        builder.Property(r => r.ParentId).HasColumnName("parent_id");
        builder.Property(r => r.StudentId).HasColumnName("student_id");
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");

        builder.HasOne(r => r.Parent)
            .WithMany(p => p.StudentRelations)
            .HasForeignKey(r => r.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Student)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("teachers", "profile");
        builder.HasKey(t => t.TeacherId);
        builder.Property(t => t.TeacherId).HasColumnName("teacher_id");
        builder.Property(t => t.CampusId).HasColumnName("campus_id");
        builder.Property(t => t.Specialty).HasColumnName("specialty").HasMaxLength(150);
        builder.Property(t => t.Bio).HasColumnName("bio");
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Teacher>(t => t.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Campus>()
            .WithMany()
            .HasForeignKey(t => t.CampusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AcademicManagerConfiguration : IEntityTypeConfiguration<AcademicManager>
{
    public void Configure(EntityTypeBuilder<AcademicManager> builder)
    {
        builder.ToTable("academic_managers", "profile");
        builder.HasKey(m => m.ManagerId);
        builder.Property(m => m.ManagerId).HasColumnName("manager_id");
        builder.Property(m => m.CampusId).HasColumnName("campus_id");
        builder.Property(m => m.CreatedAt).HasColumnName("created_at");

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<AcademicManager>(m => m.ManagerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Campus>()
            .WithMany()
            .HasForeignKey(m => m.CampusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AcademicDirectorConfiguration : IEntityTypeConfiguration<AcademicDirector>
{
    public void Configure(EntityTypeBuilder<AcademicDirector> builder)
    {
        builder.ToTable("academic_directors", "profile");
        builder.HasKey(d => d.DirectorId);
        builder.Property(d => d.DirectorId).HasColumnName("director_id");
        builder.Property(d => d.CreatedAt).HasColumnName("created_at");

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<AcademicDirector>(d => d.DirectorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.ToTable("administrators", "profile");
        builder.HasKey(a => a.AdminId);
        builder.Property(a => a.AdminId).HasColumnName("admin_id");
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Administrator>(a => a.AdminId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
