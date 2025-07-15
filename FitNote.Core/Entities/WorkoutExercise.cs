namespace FitNote.Core.Entities;

public class WorkoutExercise {
  public Guid Id { get; set; }
  public int Order { get; set; }
  public string? Notes { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  // Foreign keys
  public Guid WorkoutId { get; set; }
  public Guid ExerciseId { get; set; }

  // Navigation properties
  public virtual Workout Workout { get; set; } = null!;
  public virtual Exercise Exercise { get; set; } = null!;
  public virtual ICollection<ExerciseSet> Sets { get; set; } = new List<ExerciseSet>();
}