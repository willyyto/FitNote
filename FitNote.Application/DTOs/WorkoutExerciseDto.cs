namespace FitNote.Application.DTOs;

public class WorkoutExerciseDto {
  public Guid Id { get; set; }
  public int Order { get; set; }
  public string? Notes { get; set; }
  public Guid WorkoutId { get; set; }
  public Guid ExerciseId { get; set; }
  public ExerciseDto? Exercise { get; set; }
  public List<ExerciseSetDto> Sets { get; set; } = new();
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}