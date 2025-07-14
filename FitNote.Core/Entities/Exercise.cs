using FitNote.Core.Enums;

namespace FitNote.Core.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ExerciseCategory Category { get; set; }
    public MuscleGroup PrimaryMuscleGroup { get; set; }
    public ICollection<MuscleGroup> SecondaryMuscleGroups { get; set; } = new List<MuscleGroup>();
    public string? Instructions { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsDefault { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign keys
    public Guid? CreatedByUserId { get; set; }
    
    // Navigation properties
    public virtual User? CreatedByUser { get; set; }
    public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}
