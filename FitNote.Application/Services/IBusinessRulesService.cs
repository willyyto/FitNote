using FitNote.Application.GraphQL.Inputs;

namespace FitNote.Application.Services;

public interface IBusinessRulesService {
  Task<ValidationResult> ValidateWorkoutCreationAsync(Guid userId, CreateWorkoutInput input);
  Task<ValidationResult> ValidateExerciseAdditionAsync(Guid userId, Guid workoutId);
  Task<ValidationResult> ValidateSetAdditionAsync(Guid userId, Guid workoutExerciseId);
  Task<ValidationResult> ValidateSubscriptionLimitsAsync(Guid userId, string operation);
}

public class ValidationResult {
  public bool IsValid => !Errors.Any();
  public Dictionary<string, string> Errors { get; } = new();

  public void AddError(string key, string message) {
    Errors[key] = message;
  }
}