using FitNote.Application.GraphQL.Inputs;
using FluentValidation;

namespace FitNote.Application.Validators;

public class AddSetInputValidator : AbstractValidator<AddSetInput> {
  public AddSetInputValidator() {
    RuleFor(x => x.WorkoutExerciseId)
      .NotEmpty().WithMessage("Workout exercise ID is required");

    RuleFor(x => x.SetNumber)
      .GreaterThan(0).WithMessage("Set number must be greater than 0");

    RuleFor(x => x.Reps)
      .GreaterThan(0).WithMessage("Reps must be greater than 0")
      .When(x => x.Reps.HasValue);

    RuleFor(x => x.Weight)
      .GreaterThan(0).WithMessage("Weight must be greater than 0")
      .When(x => x.Weight.HasValue);

    RuleFor(x => x.Duration)
      .Must(duration => duration > TimeSpan.Zero)
      .WithMessage("Duration must be greater than zero")
      .When(x => x.Duration.HasValue);

    RuleFor(x => x.Distance)
      .GreaterThan(0).WithMessage("Distance must be greater than 0")
      .When(x => x.Distance.HasValue);

    RuleFor(x => x.Type)
      .IsInEnum().WithMessage("Invalid set type");

    RuleFor(x => x.Notes)
      .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
  }
}