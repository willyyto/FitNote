using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;

namespace FitNote.Application.Services;

public interface IWorkoutService
{
    Task<WorkoutDto?> GetWorkoutByIdAsync(Guid id);
    Task<IEnumerable<WorkoutDto>> GetUserWorkoutsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<WorkoutDto>> GetRecentWorkoutsAsync(Guid userId, int count = 10);
    Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutInput input, Guid userId);
    Task<WorkoutDto?> UpdateWorkoutAsync(UpdateWorkoutInput input, Guid userId);
    Task<bool> DeleteWorkoutAsync(Guid id, Guid userId);
    Task<WorkoutExerciseDto?> AddExerciseToWorkoutAsync(AddExerciseToWorkoutInput input, Guid userId);
    Task<bool> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId, Guid userId);
    Task<ExerciseSetDto?> AddSetAsync(AddSetInput input, Guid userId);
    Task<bool> DeleteSetAsync(Guid setId, Guid userId);
    Task<ExerciseSetDto?> UpdateSetAsync(Guid setId, AddSetInput input, Guid userId);
}
