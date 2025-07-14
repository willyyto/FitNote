using FitNote.Core.Enums;

namespace FitNote.Application.GraphQL.Inputs;

public class AddSetInput
{
    public Guid WorkoutExerciseId { get; set; }
    public int SetNumber { get; set; }
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public TimeSpan? Duration { get; set; }
    public decimal? Distance { get; set; }
    public SetType Type { get; set; } = SetType.Working;
    public bool IsCompleted { get; set; } = false;
    public string? Notes { get; set; }
}
