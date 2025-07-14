
using AutoMapper;
using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;
using FitNote.Core.Entities;
using FitNote.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FitNote.Application.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<WorkoutService> _logger;

    public WorkoutService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WorkoutService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<WorkoutDto?> GetWorkoutByIdAsync(Guid id)
    {
        var workout = await _unitOfWork.Workouts.GetWorkoutWithDetailsAsync(id);
        return workout != null ? _mapper.Map<WorkoutDto>(workout) : null;
    }

    public async Task<IEnumerable<WorkoutDto>> GetUserWorkoutsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var workouts = await _unitOfWork.Workouts.GetUserWorkoutsAsync(userId, startDate, endDate);
        return _mapper.Map<IEnumerable<WorkoutDto>>(workouts);
    }

    public async Task<IEnumerable<WorkoutDto>> GetRecentWorkoutsAsync(Guid userId, int count = 10)
    {
        var workouts = await _unitOfWork.Workouts.GetRecentWorkoutsAsync(userId, count);
        return _mapper.Map<IEnumerable<WorkoutDto>>(workouts);
    }

    public async Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutInput input, Guid userId)
    {
        var workout = _mapper.Map<Workout>(input);
        workout.UserId = userId;
        workout.Id = Guid.NewGuid();

        await _unitOfWork.Workouts.AddAsync(workout);
        await _unitOfWork.SaveChangesAsync();

        var createdWorkout = await _unitOfWork.Workouts.GetWorkoutWithDetailsAsync(workout.Id);
        return _mapper.Map<WorkoutDto>(createdWorkout);
    }

    public async Task<WorkoutDto?> UpdateWorkoutAsync(UpdateWorkoutInput input, Guid userId)
    {
        var workout = await _unitOfWork.Workouts.GetByIdAsync(input.Id);
        if (workout == null || workout.UserId != userId)
            return null;

        _mapper.Map(input, workout);
        _unitOfWork.Workouts.Update(workout);
        await _unitOfWork.SaveChangesAsync();

        var updatedWorkout = await _unitOfWork.Workouts.GetWorkoutWithDetailsAsync(workout.Id);
        return _mapper.Map<WorkoutDto>(updatedWorkout);
    }

    public async Task<bool> DeleteWorkoutAsync(Guid id, Guid userId)
    {
        var workout = await _unitOfWork.Workouts.GetByIdAsync(id);
        if (workout == null || workout.UserId != userId)
            return false;

        _unitOfWork.Workouts.Delete(workout);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<WorkoutExerciseDto?> AddExerciseToWorkoutAsync(AddExerciseToWorkoutInput input, Guid userId)
    {
        var workout = await _unitOfWork.Workouts.GetByIdAsync(input.WorkoutId);
        if (workout == null || workout.UserId != userId)
            return null;

        var exercise = await _unitOfWork.Exercises.GetByIdAsync(input.ExerciseId);
        if (exercise == null)
            return null;

        var workoutExercise = _mapper.Map<WorkoutExercise>(input);
        workoutExercise.Id = Guid.NewGuid();

        await _unitOfWork.Repository<WorkoutExercise>().AddAsync(workoutExercise);
        await _unitOfWork.SaveChangesAsync();

        var createdWorkoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(workoutExercise.Id);
        return _mapper.Map<WorkoutExerciseDto>(createdWorkoutExercise);
    }

    public async Task<bool> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId, Guid userId)
    {
        var workoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(workoutExerciseId);
        if (workoutExercise == null)
            return false;

        var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutExercise.WorkoutId);
        if (workout == null || workout.UserId != userId)
            return false;

        _unitOfWork.Repository<WorkoutExercise>().Delete(workoutExercise);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<ExerciseSetDto?> AddSetAsync(AddSetInput input, Guid userId)
    {
        var workoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(input.WorkoutExerciseId);
        if (workoutExercise == null)
            return null;

        var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutExercise.WorkoutId);
        if (workout == null || workout.UserId != userId)
            return null;

        var exerciseSet = _mapper.Map<ExerciseSet>(input);
        exerciseSet.Id = Guid.NewGuid();

        await _unitOfWork.Repository<ExerciseSet>().AddAsync(exerciseSet);
        await _unitOfWork.SaveChangesAsync();

        var createdSet = await _unitOfWork.Repository<ExerciseSet>().GetByIdAsync(exerciseSet.Id);
        return _mapper.Map<ExerciseSetDto>(createdSet);
    }

    public async Task<bool> DeleteSetAsync(Guid setId, Guid userId)
    {
        var exerciseSet = await _unitOfWork.Repository<ExerciseSet>().GetByIdAsync(setId);
        if (exerciseSet == null)
            return false;

        var workoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(exerciseSet.WorkoutExerciseId);
        if (workoutExercise == null)
            return false;

        var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutExercise.WorkoutId);
        if (workout == null || workout.UserId != userId)
            return false;

        _unitOfWork.Repository<ExerciseSet>().Delete(exerciseSet);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<ExerciseSetDto?> UpdateSetAsync(Guid setId, AddSetInput input, Guid userId)
    {
        var exerciseSet = await _unitOfWork.Repository<ExerciseSet>().GetByIdAsync(setId);
        if (exerciseSet == null)
            return null;

        var workoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(exerciseSet.WorkoutExerciseId);
        if (workoutExercise == null)
            return null;

        var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutExercise.WorkoutId);
        if (workout == null || workout.UserId != userId)
            return null;

        _mapper.Map(input, exerciseSet);
        exerciseSet.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<ExerciseSet>().Update(exerciseSet);
        await _unitOfWork.SaveChangesAsync();

        var updatedSet = await _unitOfWork.Repository<ExerciseSet>().GetByIdAsync(exerciseSet.Id);
        return _mapper.Map<ExerciseSetDto>(updatedSet);
    }
}
