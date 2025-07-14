using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitNote.Core.Entities;

public class UserConfig : IEntityTypeConfiguration<User> {
  public void Configure(EntityTypeBuilder<User> builder) {
    builder.ToTable(nameof(User));

    builder.Property(e => e.Id).HasValueGenerator<IdGenerator>();
    builder.Property(e => e.Username).IsRequired().HasMaxLength(100);
    builder.Property(e => e.Password).IsRequired().HasMaxLength(500);
    builder.Property(e => e.Email).IsRequired().HasMaxLength(320);
    builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
    builder.Property(e => e.Role).IsRequired().HasMaxLength(50);
    builder.Property(e => e.RefreshToken).HasMaxLength(500);

    // Enhanced properties
    builder.Property(e => e.FirstName).HasMaxLength(100);
    builder.Property(e => e.LastName).HasMaxLength(100);
    builder.Property(e => e.ProfilePictureUrl).HasMaxLength(1000);
    builder.Property(e => e.Phone).HasMaxLength(20);

    builder.Property(e => e.LastLoginIp).HasMaxLength(45);
    builder.Property(e => e.EmailVerified).HasDefaultValue(false);
    builder.Property(e => e.EmailVerificationToken).HasMaxLength(100);
    builder.Property(e => e.PasswordResetToken).HasMaxLength(100);
    builder.Property(e => e.TwoFactorEnabled).HasDefaultValue(false);
    builder.Property(e => e.TwoFactorSecret).HasMaxLength(100);
    builder.Property(e => e.Timezone).HasMaxLength(50).HasDefaultValue("UTC");
    builder.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("en");
    builder.Property(e => e.NotificationEmailEnabled).HasDefaultValue(true);
    builder.Property(e => e.NotificationSmsEnabled).HasDefaultValue(false);
    builder.Property(e => e.Preferences).HasMaxLength(5000); // JSON

    builder.HasIndex(e => e.Username).IsUnique();
    builder.HasIndex(e => e.Email).IsUnique();
    builder.HasIndex(e => e.Role);
    builder.HasIndex(e => e.EmailVerified);
    builder.HasIndex(e => e.LastLoginAt);

    builder.ConfigureMetaData().ConfigureArchivable().ConfigureActive().ConfigureLockable();
  }
}