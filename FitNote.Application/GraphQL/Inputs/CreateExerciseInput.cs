using FitNote.Core.Enums;

namespace FitNote.Application.GraphQL.Inputs;

public class CreateExerciseInput
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ExerciseCategory Category { get; set; }
    public MuscleGroup PrimaryMuscleGroup { get; set; }
    public List<MuscleGroup> SecondaryMuscleGroups { get; set; } = new();
    public string? Instructions { get; set; }
    public string? ImageUrl { get; set; }
}
