using FitNote.Core.Enums;

namespace FitNote.Application.DTOs;

public class ExerciseDto {
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public ExerciseCategory Category { get; set; }
  public MuscleGroup PrimaryMuscleGroup { get; set; }
  public List<MuscleGroup> SecondaryMuscleGroups { get; set; } = new();
  public string? Instructions { get; set; }
  public string? ImageUrl { get; set; }
  public bool IsDefault { get; set; }
  public Guid? CreatedByUserId { get; set; }
  public UserDto? CreatedByUser { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}