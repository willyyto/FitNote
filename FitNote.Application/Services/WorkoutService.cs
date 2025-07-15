using AutoMapper;
using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;
using FitNote.Core.Entities;
using FitNote.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FitNote.Application.Services;

public class WorkoutService : IWorkoutService {
  private readonly ILogger<WorkoutService> _logger;
  private readonly IMapper _mapper;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IBusinessRulesService _businessRules;

  public WorkoutService(
    IUnitOfWork unitOfWork, 
    IMapper mapper, 
    ILogger<WorkoutService> logger,
    IBusinessRulesService businessRules) {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _logger = logger;
    _businessRules = businessRules;
  }

  public async Task<WorkoutDto?> GetWorkoutByIdAsync(Guid id) {
    try {
      var workout = await _unitOfWork.Workouts.GetWorkoutWithDetailsAsync(id);
      return workout != null ? _mapper.Map<WorkoutDto>(workout) : null;
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error getting workout by ID: {WorkoutId}", id);
      throw;
    }
  }

  public async Task<IEnumerable<WorkoutDto>> GetUserWorkoutsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null) {
    try {
      var workouts = await _unitOfWork.Workouts.GetUserWorkoutsAsync(userId, startDate, endDate);
      return _mapper.Map<IEnumerable<WorkoutDto>>(workouts);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error getting user workouts for user: {UserId}", userId);
      throw;
    }
  }

  public async Task<IEnumerable<WorkoutDto>> GetRecentWorkoutsAsync(Guid userId, int count = 10) {
    try {
      var workouts = await _unitOfWork.Workouts.GetRecentWorkoutsAsync(userId, count);
      return _mapper.Map<IEnumerable<WorkoutDto>>(workouts);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error getting recent workouts for user: {UserId}", userId);
      throw;
    }
  }

  public async Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutInput input, Guid userId) {
    try {
      // Validate business rules
      var validation = await _businessRules.ValidateWorkoutCreationAsync(userId, input);
      if (!validation.IsValid) {
        throw new InvalidOperationException($"Validation failed: {string.Join(", ", validation.Errors.Values)}");
      }

      // Check subscription limits
      var subscriptionValidation = await _businessRules.ValidateSubscriptionLimitsAsync(userId, "create_workout");
      if (!subscriptionValidation.IsValid) {
        throw new InvalidOperationException($"Subscription limit exceeded: {string.Join(", ", subscriptionValidation.Errors.Values)}");
      }

      var workout = _mapper.Map<Workout>(input);
      workout.UserId = userId;
      workout.Id = Guid.NewGuid();
      workout.CreatedAt = DateTime.UtcNow;

      await _unitOfWork.Workouts.AddAsync(workout);
      await _unitOfWork.SaveChangesAsync();

      _logger.LogInformation("Workout created successfully: {WorkoutId} for user: {UserId}", workout.Id, userId);

      var createdWorkout = await _unitOfWork.Workouts.GetWorkoutWithDetailsAsync(workout.Id);
      return _mapper.Map<WorkoutDto>(createdWorkout);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error creating workout for user: {UserId}", userId);
      throw;
    }
  }

  public async Task<WorkoutDto?> UpdateWorkoutAsync(UpdateWorkoutInput input, Guid userId) {
    try {
      var workout = await _unitOfWork.Workouts.GetByIdAsync(input.Id);
      if (workout == null || workout.UserId != userId) {
        _logger.LogWarning("Workout not found or access denied: {WorkoutId} for user: {UserId}", input.Id, userId);
        return null;
      }

      _mapper.Map(input, workout);
      workout.UpdatedAt = DateTime.UtcNow;
      
      _unitOfWork.Workouts.Update(workout);
      await _unitOfWork.SaveChangesAsync();

      _logger.LogInformation("Workout updated successfully: {WorkoutId}", workout.Id);

      var updatedWorkout = await _unitOfWork.Workouts.GetWorkoutWithDetailsAsync(workout.Id);
      return _mapper.Map<WorkoutDto>(updatedWorkout);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error updating workout: {WorkoutId}", input.Id);
      throw;
    }
  }

  public async Task<bool> DeleteWorkoutAsync(Guid id, Guid userId) {
    try {
      var workout = await _unitOfWork.Workouts.GetByIdAsync(id);
      if (workout == null || workout.UserId != userId) {
        _logger.LogWarning("Workout not found or access denied: {WorkoutId} for user: {UserId}", id, userId);
        return false;
      }

      _unitOfWork.Workouts.Delete(workout);
      await _unitOfWork.SaveChangesAsync();

      _logger.LogInformation("Workout deleted successfully: {WorkoutId}", id);
      return true;
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error deleting workout: {WorkoutId}", id);
      throw;
    }
  }

  public async Task<WorkoutExerciseDto?> AddExerciseToWorkoutAsync(AddExerciseToWorkoutInput input, Guid userId) {
    try {
      // Validate business rules
      var validation = await _businessRules.ValidateExerciseAdditionAsync(userId, input.WorkoutId);
      if (!validation.IsValid) {
        throw new InvalidOperationException($"Validation failed: {string.Join(", ", validation.Errors.Values)}");
      }

      var exercise = await _unitOfWork.Exercises.GetByIdAsync(input.ExerciseId);
      if (exercise == null) {
        _logger.LogWarning("Exercise not found: {ExerciseId}", input.ExerciseId);
        return null;
      }

      var workoutExercise = _mapper.Map<WorkoutExercise>(input);
      workoutExercise.Id = Guid.NewGuid();
      workoutExercise.CreatedAt = DateTime.UtcNow;

      await _unitOfWork.Repository<WorkoutExercise>().AddAsync(workoutExercise);
      await _unitOfWork.SaveChangesAsync();

      _logger.LogInformation("Exercise added to workout: {ExerciseId} to {WorkoutId}", input.ExerciseId, input.WorkoutId);

      var createdWorkoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(workoutExercise.Id);
      return _mapper.Map<WorkoutExerciseDto>(createdWorkoutExercise);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error adding exercise to workout: {ExerciseId} to {WorkoutId}", input.ExerciseId, input.WorkoutId);
      throw;
    }
  }

  public async Task<bool> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId, Guid userId) {
    try {
      var workoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(workoutExerciseId);
      if (workoutExercise == null) {
        _logger.LogWarning("Workout exercise not found: {WorkoutExerciseId}", workoutExerciseId);
        return false;
      }

      var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutExercise.WorkoutId);
      if (workout == null || workout.UserId != userId) {
        _logger.LogWarning("Workout not found or access denied for workout exercise: {WorkoutExerciseId}", workoutExerciseId);
        return false;
      }

      _unitOfWork.Repository<WorkoutExercise>().Delete(workoutExercise);
      await _unitOfWork.SaveChangesAsync();

      _logger.LogInformation("Exercise removed from workout: {WorkoutExerciseId}", workoutExerciseId);
      return true;
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error removing exercise from workout: {WorkoutExerciseId}", workoutExerciseId);
      throw;
    }
  }

  public async Task<ExerciseSetDto?> AddSetAsync(AddSetInput input, Guid userId) {
    try {
      // Validate business rules
      var validation = await _businessRules.ValidateSetAdditionAsync(userId, input.WorkoutExerciseId);
      if (!validation.IsValid) {
        throw new InvalidOperationException($"Validation failed: {string.Join(", ", validation.Errors.Values)}");
      }

      var exerciseSet = _mapper.Map<ExerciseSet>(input);
      exerciseSet.Id = Guid.NewGuid();
      exerciseSet.CreatedAt = DateTime.UtcNow;

      await _unitOfWork.Repository<ExerciseSet>().AddAsync(exerciseSet);
      await _unitOfWork.SaveChangesAsync();

      _logger.LogInformation("Set added to workout exercise: {WorkoutExerciseId}", input.WorkoutExerciseId);

      var createdSet = await _unitOfWork.Repository<ExerciseSet>().GetByIdAsync(exerciseSet.Id);
      return _mapper.Map<ExerciseSetDto>(createdSet);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error adding set to workout exercise: {WorkoutExerciseId}", input.WorkoutExerciseId);
      throw;
    }
  }

  public async Task<bool> DeleteSetAsync(Guid setId, Guid userId) {
    try {
      var exerciseSet = await _unitOfWork.Repository<ExerciseSet>().GetByIdAsync(setId);
      if (exerciseSet == null) {
        _logger.LogWarning("Exercise set not found: {SetId}", setId);
        return false;
      }

      var workoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(exerciseSet.WorkoutExerciseId);
      if (workoutExercise == null) {
        _logger.LogWarning("Workout exercise not found for set: {SetId}", setId);
        return false;
      }

      var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutExercise.WorkoutId);
      if (workout == null || workout.UserId != userId) {
        _logger.LogWarning("Workout not found or access denied for set: {SetId}", setId);
        return false;
      }

      _unitOfWork.Repository<ExerciseSet>().Delete(exerciseSet);
      await _unitOfWork.SaveChangesAsync();

      _logger.LogInformation("Set deleted successfully: {SetId}", setId);
      return true;
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error deleting set: {SetId}", setId);
      throw;
    }
  }

  public async Task<ExerciseSetDto?> UpdateSetAsync(Guid setId, AddSetInput input, Guid userId) {
    try {
      var exerciseSet = await _unitOfWork.Repository<ExerciseSet>().GetByIdAsync(setId);
      if (exerciseSet == null) {
        _logger.LogWarning("Exercise set not found: {SetId}", setId);
        return null;
      }

      var workoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(exerciseSet.WorkoutExerciseId);
      if (workoutExercise == null) {
        _logger.LogWarning("Workout exercise not found for set: {SetId}", setId);
        return null;
      }

      var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutExercise.WorkoutId);
      if (workout == null || workout.UserId != userId) {
        _logger.LogWarning("Workout not found or access denied for set: {SetId}", setId);
        return null;
      }

      _mapper.Map(input, exerciseSet);
      exerciseSet.UpdatedAt = DateTime.UtcNow;

      _unitOfWork.Repository<ExerciseSet>().Update(exerciseSet);
      await _unitOfWork.SaveChangesAsync();

      _logger.LogInformation("Set updated successfully: {SetId}", setId);

      var updatedSet = await _unitOfWork.Repository<ExerciseSet>().GetByIdAsync(exerciseSet.Id);
      return _mapper.Map<ExerciseSetDto>(updatedSet);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "Error updating set: {SetId}", setId);
      throw;
    }
  }
}