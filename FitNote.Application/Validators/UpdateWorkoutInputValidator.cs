using FitNote.Application.GraphQL.Inputs;
using FluentValidation;

namespace FitNote.Application.Validators;

public class UpdateWorkoutInputValidator : AbstractValidator<UpdateWorkoutInput> {
  public UpdateWorkoutInputValidator() {
    RuleFor(x => x.Id)
      .NotEmpty().WithMessage("Workout ID is required");

    RuleFor(x => x.Name)
      .MaximumLength(100).WithMessage("Workout name must not exceed 100 characters")
      .When(x => !string.IsNullOrEmpty(x.Name));

    RuleFor(x => x.Notes)
      .MaximumLength(500).WithMessage("Notes must not exceed 500 characters")
      .When(x => !string.IsNullOrEmpty(x.Notes));

    RuleFor(x => x.Date)
      .Must(date => date <= DateTime.Now.AddDays(30))
      .WithMessage("Date cannot be more than 30 days in the future")
      .When(x => x.Date.HasValue);

    RuleFor(x => x.Status)
      .IsInEnum().WithMessage("Invalid workout status")
      .When(x => x.Status.HasValue);

    RuleFor(x => x.Duration)
      .Must(duration => duration >= TimeSpan.Zero)
      .WithMessage("Duration must be positive")
      .When(x => x.Duration.HasValue);
  }
}