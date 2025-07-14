namespace FitNote.Application.GraphQL.Inputs;

public class AddExerciseToWorkoutInput
{
    public Guid WorkoutId { get; set; }
    public Guid ExerciseId { get; set; }
    public int Order { get; set; }
    public string? Notes { get; set; }
}
