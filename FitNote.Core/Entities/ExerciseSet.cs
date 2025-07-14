using FitNote.Core.Enums;

namespace FitNote.Core.Entities;

public class ExerciseSet
{
    public Guid Id { get; set; }
    public int SetNumber { get; set; }
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public TimeSpan? Duration { get; set; }
    public decimal? Distance { get; set; }
    public SetType SetType { get; set; } = SetType.Working;
    public bool IsCompleted { get; set; } = false;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign keys
    public Guid WorkoutExerciseId { get; set; }
    
    // Navigation properties
    public virtual WorkoutExercise WorkoutExercise { get; set; } = null!;
}
