using FitNote.Application.GraphQL.Inputs;
using FitNote.Core.Enums;
using FluentValidation;

namespace FitNote.Application.Validators;

public class CreateExerciseInputValidator : AbstractValidator<CreateExerciseInput> {
  public CreateExerciseInputValidator() {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Exercise name is required")
      .MaximumLength(100).WithMessage("Exercise name must not exceed 100 characters");

    RuleFor(x => x.Description)
      .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

    RuleFor(x => x.Category)
      .IsInEnum().WithMessage("Invalid exercise category");

    RuleFor(x => x.PrimaryMuscleGroup)
      .IsInEnum().WithMessage("Invalid primary muscle group");

    RuleFor(x => x.SecondaryMuscleGroups)
      .Must(groups => groups == null || groups.All(g => Enum.IsDefined(typeof(MuscleGroup), g)))
      .WithMessage("Invalid secondary muscle groups");

    RuleFor(x => x.Instructions)
      .MaximumLength(2000).WithMessage("Instructions must not exceed 2000 characters");

    RuleFor(x => x.ImageUrl)
      .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
      .WithMessage("Image URL must be a valid URL");
  }
}