using FitNote.Application.GraphQL.Inputs;
using FluentValidation;

namespace FitNote.Application.Validators;

public class RegisterInputValidator : AbstractValidator<RegisterInput> {
  public RegisterInputValidator() {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Valid email is required")
      .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
      .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
      .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit");

    RuleFor(x => x.FirstName)
      .NotEmpty().WithMessage("First name is required")
      .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

    RuleFor(x => x.LastName)
      .NotEmpty().WithMessage("Last name is required")
      .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

    RuleFor(x => x.UserName)
      .NotEmpty().WithMessage("Username is required")
      .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
      .MaximumLength(50).WithMessage("Username must not exceed 50 characters")
      .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");
  }
}