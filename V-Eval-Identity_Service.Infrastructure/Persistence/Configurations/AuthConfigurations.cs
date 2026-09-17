using Domain.Entities.Iam;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

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

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles", "iam");
        builder.HasKey(r => r.RoleId);
        builder.Property(r => r.RoleId).HasColumnName("role_id");
        builder.HasIndex(r => r.RoleName).IsUnique();
        builder.Property(r => r.RoleName).HasColumnName("role_name").HasMaxLength(50).IsRequired();
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
    }
}
