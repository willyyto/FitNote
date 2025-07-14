namespace FitNote.Core.Entities;

public class User : IArchivable, IMetaData, IActive, ILockable {
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public string Email { get; set; }
  public string Role { get; set; }
  public string RefreshToken { get; set; }
  public DateTime TokenCreated { get; set; }
  public DateTime TokenExpires { get; set; }

  // Enhanced user features
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? ProfilePictureUrl { get; set; }
  public string? Phone { get; set; }
  public string? Department { get; set; }
  public string? JobTitle { get; set; }
  public DateTime? LastLoginAt { get; set; }
  public string? LastLoginIp { get; set; }
  public bool EmailVerified { get; set; } = false;
  public string? EmailVerificationToken { get; set; }
  public DateTime? EmailVerificationExpires { get; set; }
  public string? PasswordResetToken { get; set; }
  public DateTime? PasswordResetExpires { get; set; }
  public bool TwoFactorEnabled { get; set; } = false;
  public string? TwoFactorSecret { get; set; }
  public string? Timezone { get; set; } = "UTC";
  public string? Language { get; set; } = "en";
  public bool NotificationEmailEnabled { get; set; } = true;
  public bool NotificationSmsEnabled { get; set; } = false;
  public string? Preferences { get; set; } // JSON for user preferences

  public bool IsActive { get; set; }
  public bool IsArchived { get; set; }
  public bool IsLocked { get; set; }
  public DateTime UpdatedOn { get; set; }
  public DateTime CreatedOn { get; set; }
}