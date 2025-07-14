using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;
using FitNote.Core.Enums;

namespace FitNote.Application.Services;

public interface IExerciseService
{
    Task<ExerciseDto?> GetExerciseByIdAsync(Guid id);
    Task<IEnumerable<ExerciseDto>> GetAllExercisesAsync();
    Task<IEnumerable<ExerciseDto>> GetExercisesByCategoryAsync(ExerciseCategory category);
    Task<IEnumerable<ExerciseDto>> GetExercisesByMuscleGroupAsync(MuscleGroup muscleGroup);
    Task<IEnumerable<ExerciseDto>> GetUserExercisesAsync(Guid userId);
    Task<IEnumerable<ExerciseDto>> GetDefaultExercisesAsync();
    Task<IEnumerable<ExerciseDto>> SearchExercisesAsync(string searchTerm);
    Task<ExerciseDto> CreateExerciseAsync(CreateExerciseInput input, Guid userId);
    Task<bool> DeleteExerciseAsync(Guid id, Guid userId);
}
