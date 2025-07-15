using FitNote.Application.GraphQL.Inputs;
using FitNote.Core.Entities;
using FitNote.Core.Enums;
using FitNote.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FitNote.Application.Services;

public class BusinessRulesService : IBusinessRulesService {
  private readonly IUnitOfWork _unitOfWork;
  private readonly IConfiguration _configuration;
  private readonly ILogger<BusinessRulesService> _logger;

  public BusinessRulesService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<BusinessRulesService> logger) {
    _unitOfWork = unitOfWork;
    _configuration = configuration;
    _logger = logger;
  }

  public async Task<ValidationResult> ValidateWorkoutCreationAsync(Guid userId, CreateWorkoutInput input) {
    var result = new ValidationResult();

    // Check max workouts per user
    var maxWorkouts = _configuration.GetValue<int>("Features:MaxWorkoutsPerUser", 1000);
    var currentWorkoutCount = await _unitOfWork.Repository<Workout>()
      .CountAsync(w => w.UserId == userId);

    if (currentWorkoutCount >= maxWorkouts) {
      result.AddError("workout_limit", $"Maximum number of workouts ({maxWorkouts}) reached");
    }

    // Check date validity
    if (input.Date > DateTime.UtcNow.AddDays(365)) {
      result.AddError("date_invalid", "Workout date cannot be more than 1 year in the future");
    }

    return result;
  }

  public async Task<ValidationResult> ValidateExerciseAdditionAsync(Guid userId, Guid workoutId) {
    var result = new ValidationResult();

    var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutId);
    if (workout?.UserId != userId) {
      result.AddError("workout_access", "Workout not found or access denied");
      return result;
    }

    // Check max exercises per workout
    var maxExercisesPerWorkout = _configuration.GetValue<int>("Features:MaxExercisesPerWorkout", 50);
    var currentExerciseCount = await _unitOfWork.Repository<WorkoutExercise>()
      .CountAsync(we => we.WorkoutId == workoutId);

    if (currentExerciseCount >= maxExercisesPerWorkout) {
      result.AddError("exercise_limit", $"Maximum number of exercises per workout ({maxExercisesPerWorkout}) reached");
    }

    return result;
  }

  public async Task<ValidationResult> ValidateSetAdditionAsync(Guid userId, Guid workoutExerciseId) {
    var result = new ValidationResult();

    var workoutExercise = await _unitOfWork.Repository<WorkoutExercise>().GetByIdAsync(workoutExerciseId);
    if (workoutExercise == null) {
      result.AddError("workout_exercise_not_found", "Workout exercise not found");
      return result;
    }

    var workout = await _unitOfWork.Workouts.GetByIdAsync(workoutExercise.WorkoutId);
    if (workout?.UserId != userId) {
      result.AddError("workout_access", "Workout not found or access denied");
      return result;
    }

    // Check max sets per exercise
    var maxSetsPerExercise = _configuration.GetValue<int>("Features:MaxSetsPerExercise", 100);
    var currentSetCount = await _unitOfWork.Repository<ExerciseSet>()
      .CountAsync(s => s.WorkoutExerciseId == workoutExerciseId);

    if (currentSetCount >= maxSetsPerExercise) {
      result.AddError("set_limit", $"Maximum number of sets per exercise ({maxSetsPerExercise}) reached");
    }

    return result;
  }

  public async Task<ValidationResult> ValidateSubscriptionLimitsAsync(Guid userId, string operation) {
    var result = new ValidationResult();

    var subscription = await _unitOfWork.Repository<UserSubscription>()
      .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);

    var tier = subscription?.Tier ?? SubscriptionTier.Free;

    // Define subscription limits
    var limits = tier switch {
      SubscriptionTier.Free => new { MaxWorkouts = 10, MaxCustomExercises = 5 },
      SubscriptionTier.Premium => new { MaxWorkouts = 100, MaxCustomExercises = 50 },
      SubscriptionTier.Pro => new { MaxWorkouts = -1, MaxCustomExercises = -1 } // Unlimited
    };

    switch (operation) {
      case "create_workout":
        if (limits.MaxWorkouts > 0) {
          var workoutCount = await _unitOfWork.Repository<Workout>()
            .CountAsync(w => w.UserId == userId);
          if (workoutCount >= limits.MaxWorkouts) {
            result.AddError("subscription_limit", $"Your {tier} subscription allows up to {limits.MaxWorkouts} workouts");
          }
        }
        break;

      case "create_exercise":
        if (limits.MaxCustomExercises > 0) {
          var exerciseCount = await _unitOfWork.Repository<Exercise>()
            .CountAsync(e => e.CreatedByUserId == userId);
          if (exerciseCount >= limits.MaxCustomExercises) {
            result.AddError("subscription_limit", $"Your {tier} subscription allows up to {limits.MaxCustomExercises} custom exercises");
          }
        }
        break;
    }

    return result;
  }
}