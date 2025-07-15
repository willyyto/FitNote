using Microsoft.AspNetCore.Identity;

namespace FitNote.Core.Entities;

public class User : IdentityUser<Guid> {
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public bool IsActive { get; set; } = true;
  public string? ProfilePictureUrl { get; set; }
  public DateTime? LastLoginAt { get; set; }

  // Navigation properties
  public virtual ICollection<Workout> Workouts { get; set; } = new List<Workout>();
  public virtual ICollection<Exercise> CreatedExercises { get; set; } = new List<Exercise>();
  public virtual UserSubscription? Subscription { get; set; }
}