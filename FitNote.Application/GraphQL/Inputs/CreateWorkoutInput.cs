using FitNote.Core.Enums;

namespace FitNote.Application.GraphQL.Inputs;

public class CreateWorkoutInput
{
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public WorkoutStatus Status { get; set; } = WorkoutStatus.Planned;
}
