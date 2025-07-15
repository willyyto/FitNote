using System.Security.Claims;
using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;
using FitNote.Application.Services;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;

namespace FitNote.Application.GraphQL.Mutations;

public class Mutation {
  // Authentication Mutations
  [EnableRateLimiting("AuthPolicy")]
  public async Task<AuthResult> Login(
    LoginInput input,
    [Service] IAuthService authService) {
    return await authService.LoginAsync(input);
  }

  [EnableRateLimiting("AuthPolicy")]
  public async Task<AuthResult> Register(
    RegisterInput input,
    [Service] IAuthService authService) {
    return await authService.RegisterAsync(input);
  }

  [Authorize]
  public async Task<bool> Logout(
    [Service] IAuthService authService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return false;

    return await authService.LogoutAsync(userId);
  }

  // Workout Mutations with Enhanced Error Handling
  [Authorize]
  public async Task<WorkoutDto> CreateWorkout(
    CreateWorkoutInput input,
    [Service] IWorkoutService workoutService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized workout creation attempt");
      throw new UnauthorizedAccessException("User not authenticated");
    }

    try {
      return await workoutService.CreateWorkoutAsync(input, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error creating workout for user: {UserId}", userId);
      throw;
    }
  }

  [Authorize]
  public async Task<WorkoutDto?> UpdateWorkout(
    UpdateWorkoutInput input,
    [Service] IWorkoutService workoutService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized workout update attempt for workout: {WorkoutId}", input.Id);
      return null;
    }

    try {
      return await workoutService.UpdateWorkoutAsync(input, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error updating workout: {WorkoutId} for user: {UserId}", input.Id, userId);
      throw;
    }
  }

  [Authorize]
  public async Task<bool> DeleteWorkout(
    Guid id,
    [Service] IWorkoutService workoutService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized workout deletion attempt for workout: {WorkoutId}", id);
      return false;
    }

    try {
      return await workoutService.DeleteWorkoutAsync(id, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error deleting workout: {WorkoutId} for user: {UserId}", id, userId);
      throw;
    }
  }

  [Authorize]
  public async Task<WorkoutExerciseDto?> AddExerciseToWorkout(
    AddExerciseToWorkoutInput input,
    [Service] IWorkoutService workoutService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized exercise addition attempt to workout: {WorkoutId}", input.WorkoutId);
      return null;
    }

    try {
      return await workoutService.AddExerciseToWorkoutAsync(input, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error adding exercise: {ExerciseId} to workout: {WorkoutId} for user: {UserId}", 
        input.ExerciseId, input.WorkoutId, userId);
      throw;
    }
  }

  [Authorize]
  public async Task<bool> RemoveExerciseFromWorkout(
    Guid workoutExerciseId,
    [Service] IWorkoutService workoutService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized exercise removal attempt: {WorkoutExerciseId}", workoutExerciseId);
      return false;
    }

    try {
      return await workoutService.RemoveExerciseFromWorkoutAsync(workoutExerciseId, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error removing exercise from workout: {WorkoutExerciseId} for user: {UserId}", 
        workoutExerciseId, userId);
      throw;
    }
  }

  [Authorize]
  public async Task<ExerciseSetDto?> AddSet(
    AddSetInput input,
    [Service] IWorkoutService workoutService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized set addition attempt to workout exercise: {WorkoutExerciseId}", 
        input.WorkoutExerciseId);
      return null;
    }

    try {
      return await workoutService.AddSetAsync(input, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error adding set to workout exercise: {WorkoutExerciseId} for user: {UserId}", 
        input.WorkoutExerciseId, userId);
      throw;
    }
  }

  [Authorize]
  public async Task<ExerciseSetDto?> UpdateSet(
    Guid setId,
    AddSetInput input,
    [Service] IWorkoutService workoutService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized set update attempt: {SetId}", setId);
      return null;
    }

    try {
      return await workoutService.UpdateSetAsync(setId, input, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error updating set: {SetId} for user: {UserId}", setId, userId);
      throw;
    }
  }

  [Authorize]
  public async Task<bool> DeleteSet(
    Guid setId,
    [Service] IWorkoutService workoutService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized set deletion attempt: {SetId}", setId);
      return false;
    }

    try {
      return await workoutService.DeleteSetAsync(setId, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error deleting set: {SetId} for user: {UserId}", setId, userId);
      throw;
    }
  }

  // Exercise Mutations with Enhanced Business Logic
  [Authorize]
  public async Task<ExerciseDto> CreateExercise(
    CreateExerciseInput input,
    [Service] IExerciseService exerciseService,
    [Service] IBusinessRulesService businessRules,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized exercise creation attempt");
      throw new UnauthorizedAccessException("User not authenticated");
    }

    try {
      // Check subscription limits
      var validation = await businessRules.ValidateSubscriptionLimitsAsync(userId.Value, "create_exercise");
      if (!validation.IsValid) {
        throw new InvalidOperationException($"Subscription limit exceeded: {string.Join(", ", validation.Errors.Values)}");
      }

      return await exerciseService.CreateExerciseAsync(input, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error creating exercise for user: {UserId}", userId);
      throw;
    }
  }

  [Authorize]
  public async Task<bool> DeleteExercise(
    Guid id,
    [Service] IExerciseService exerciseService,
    [Service] ILogger<Mutation> logger,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) {
      logger.LogWarning("Unauthorized exercise deletion attempt: {ExerciseId}", id);
      return false;
    }

    try {
      return await exerciseService.DeleteExerciseAsync(id, userId.Value);
    }
    catch (Exception ex) {
      logger.LogError(ex, "Error deleting exercise: {ExerciseId} for user: {UserId}", id, userId);
      throw;
    }
  }

  private static Guid? GetUserId(ClaimsPrincipal claimsPrincipal) {
    var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
  }
}