using FitNote.Application.GraphQL.Inputs;
using FluentValidation;

namespace FitNote.Application.Validators;

public class CreateWorkoutInputValidator : AbstractValidator<CreateWorkoutInput> {
  public CreateWorkoutInputValidator() {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Workout name is required")
      .MaximumLength(100).WithMessage("Workout name must not exceed 100 characters");

    RuleFor(x => x.Notes)
      .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");

    RuleFor(x => x.Date)
      .NotEmpty().WithMessage("Date is required")
      .Must(date => date <= DateTime.Now.AddDays(30))
      .WithMessage("Date cannot be more than 30 days in the future");

    RuleFor(x => x.Status)
      .IsInEnum().WithMessage("Invalid workout status");
  }
}