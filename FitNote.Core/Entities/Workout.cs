using FitNote.Core.Enums;

namespace FitNote.Core.Entities;

public class Workout {
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Notes { get; set; }
  public DateTime Date { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public TimeSpan? Duration { get; set; }
  public WorkoutStatus Status { get; set; } = WorkoutStatus.Planned;

  // Foreign keys
  public Guid UserId { get; set; }

  // Navigation properties
  public virtual User User { get; set; } = null!;
  public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}