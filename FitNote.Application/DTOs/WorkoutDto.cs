using FitNote.Core.Enums;

namespace FitNote.Application.DTOs;

public class WorkoutDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? Duration { get; set; }
    public WorkoutStatus Status { get; set; }
    public Guid UserId { get; set; }
    public UserDto? User { get; set; }
    public List<WorkoutExerciseDto> WorkoutExercises { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
