using System.Security.Claims;
using FitNote.Application.DTOs;
using FitNote.Application.GraphQL.Inputs;
using FitNote.Application.Services;
using HotChocolate.Authorization;

namespace FitNote.Application.GraphQL.Mutations;

public class Mutation {
  // Authentication Mutations
  public async Task<AuthResult> Login(
    LoginInput input,
    [Service] IAuthService authService) {
    return await authService.LoginAsync(input);
  }

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

  // Workout Mutations
  [Authorize]
  public async Task<WorkoutDto> CreateWorkout(
    CreateWorkoutInput input,
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) throw new UnauthorizedAccessException("User not authenticated");

    return await workoutService.CreateWorkoutAsync(input, userId.Value);
  }

  [Authorize]
  public async Task<WorkoutDto?> UpdateWorkout(
    UpdateWorkoutInput input,
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return null;

    return await workoutService.UpdateWorkoutAsync(input, userId.Value);
  }

  [Authorize]
  public async Task<bool> DeleteWorkout(
    Guid id,
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return false;

    return await workoutService.DeleteWorkoutAsync(id, userId.Value);
  }

  [Authorize]
  public async Task<WorkoutExerciseDto?> AddExerciseToWorkout(
    AddExerciseToWorkoutInput input,
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return null;

    return await workoutService.AddExerciseToWorkoutAsync(input, userId.Value);
  }

  [Authorize]
  public async Task<bool> RemoveExerciseFromWorkout(
    Guid workoutExerciseId,
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return false;

    return await workoutService.RemoveExerciseFromWorkoutAsync(workoutExerciseId, userId.Value);
  }

  [Authorize]
  public async Task<ExerciseSetDto?> AddSet(
    AddSetInput input,
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return null;

    return await workoutService.AddSetAsync(input, userId.Value);
  }

  [Authorize]
  public async Task<ExerciseSetDto?> UpdateSet(
    Guid setId,
    AddSetInput input,
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return null;

    return await workoutService.UpdateSetAsync(setId, input, userId.Value);
  }

  [Authorize]
  public async Task<bool> DeleteSet(
    Guid setId,
    [Service] IWorkoutService workoutService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return false;

    return await workoutService.DeleteSetAsync(setId, userId.Value);
  }

  // Exercise Mutations
  [Authorize]
  public async Task<ExerciseDto> CreateExercise(
    CreateExerciseInput input,
    [Service] IExerciseService exerciseService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) throw new UnauthorizedAccessException("User not authenticated");

    return await exerciseService.CreateExerciseAsync(input, userId.Value);
  }

  [Authorize]
  public async Task<bool> DeleteExercise(
    Guid id,
    [Service] IExerciseService exerciseService,
    ClaimsPrincipal claimsPrincipal) {
    var userId = GetUserId(claimsPrincipal);
    if (userId == null) return false;

    return await exerciseService.DeleteExerciseAsync(id, userId.Value);
  }

  private static Guid? GetUserId(ClaimsPrincipal claimsPrincipal) {
    var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
  }
}