using FitNote.Core.Enums;

namespace FitNote.Application.GraphQL.Inputs;

public class UpdateWorkoutInput
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Notes { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? Duration { get; set; }
    public WorkoutStatus? Status { get; set; }
}
