using System.Security.Claims;
using FitNote.Application.DTOs;
using FitNote.Application.Services;
using FitNote.Core.Enums;
using HotChocolate.Authorization;

namespace FitNote.Application.GraphQL.Queries;

public class Query {
  // User Queries
  [Authorize]
  public async Task<UserDto?> GetCurrentUser(
    [Service] IAuthService authService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null) return null;

    return await authService.GetCurrentUserAsync(userId);
  }

  // Workout Queries
  [Authorize]
  public async Task<WorkoutDto?> GetWorkout(
    Guid id,
    [Service] IWorkoutService workoutService) {
    return await workoutService.GetWorkoutByIdAsync(id);
  }

  [Authorize]
  public async Task<IEnumerable<WorkoutDto>> GetMyWorkouts(
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal,
    DateTime? startDate = null,
    DateTime? endDate = null) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return Enumerable.Empty<WorkoutDto>();

    return await workoutService.GetUserWorkoutsAsync(userId.Value, startDate, endDate);
  }

  [Authorize]
  public async Task<IEnumerable<WorkoutDto>> GetRecentWorkouts(
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal,
    int count = 10) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return Enumerable.Empty<WorkoutDto>();

    return await workoutService.GetRecentWorkoutsAsync(userId.Value, count);
  }

  // Exercise Queries
  public async Task<ExerciseDto?> GetExercise(
    Guid id,
    [Service] IExerciseService exerciseService) {
    return await exerciseService.GetExerciseByIdAsync(id);
  }

  public async Task<IEnumerable<ExerciseDto>> GetExercises(
    [Service] IExerciseService exerciseService) {
    return await exerciseService.GetAllExercisesAsync();
  }

  public async Task<IEnumerable<ExerciseDto>> GetExercisesByCategory(
    ExerciseCategory category,
    [Service] IExerciseService exerciseService) {
    return await exerciseService.GetExercisesByCategoryAsync(category);
  }

  public async Task<IEnumerable<ExerciseDto>> GetExercisesByMuscleGroup(
    MuscleGroup muscleGroup,
    [Service] IExerciseService exerciseService) {
    return await exerciseService.GetExercisesByMuscleGroupAsync(muscleGroup);
  }

  public async Task<IEnumerable<ExerciseDto>> GetDefaultExercises(
    [Service] IExerciseService exerciseService) {
    return await exerciseService.GetDefaultExercisesAsync();
  }

  [Authorize]
  public async Task<IEnumerable<ExerciseDto>> GetMyExercises(
    [Service] IExerciseService exerciseService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return Enumerable.Empty<ExerciseDto>();

    return await exerciseService.GetUserExercisesAsync(userId.Value);
  }

  public async Task<IEnumerable<ExerciseDto>> SearchExercises(
    string searchTerm,
    [Service] IExerciseService exerciseService) {
    if (string.IsNullOrWhiteSpace(searchTerm))
      return Enumerable.Empty<ExerciseDto>();

    return await exerciseService.SearchExercisesAsync(searchTerm);
  }

  private static Guid? GetUserId(ClaimsPrincipal claimsPrincipal) {
    var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
  }
}