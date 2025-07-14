using FitNote.Core.Entities;
using FitNote.Core.Enums;

namespace FitNote.Core.Interfaces;

public interface IExerciseRepository : IRepository<Exercise>
{
    Task<IEnumerable<Exercise>> GetExercisesByCategoryAsync(ExerciseCategory category);
    Task<IEnumerable<Exercise>> GetExercisesByMuscleGroupAsync(MuscleGroup muscleGroup);
    Task<IEnumerable<Exercise>> GetUserExercisesAsync(Guid userId);
    Task<IEnumerable<Exercise>> GetDefaultExercisesAsync();
    Task<IEnumerable<Exercise>> SearchExercisesAsync(string searchTerm);
}
