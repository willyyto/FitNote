using FitNote.Core.Enums;

namespace FitNote.Application.DTOs;

public class ExerciseSetDto {
  public Guid Id { get; set; }
  public int SetNumber { get; set; }
  public int? Reps { get; set; }
  public decimal? Weight { get; set; }
  public TimeSpan? Duration { get; set; }
  public decimal? Distance { get; set; }
  public SetType Type { get; set; }
  public bool IsCompleted { get; set; }
  public string? Notes { get; set; }
  public Guid WorkoutExerciseId { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}