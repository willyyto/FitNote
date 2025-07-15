using FitNote.Application.GraphQL.Inputs;
using FluentValidation;

namespace FitNote.Application.Validators;

public class AddExerciseToWorkoutInputValidator : AbstractValidator<AddExerciseToWorkoutInput> {
  public AddExerciseToWorkoutInputValidator() {
    RuleFor(x => x.WorkoutId)
      .NotEmpty().WithMessage("Workout ID is required");

    RuleFor(x => x.ExerciseId)
      .NotEmpty().WithMessage("Exercise ID is required");

    RuleFor(x => x.Order)
      .GreaterThan(0).WithMessage("Order must be greater than 0");

    RuleFor(x => x.Notes)
      .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
  }
}