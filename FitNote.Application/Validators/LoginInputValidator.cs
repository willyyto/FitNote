using FitNote.Application.GraphQL.Inputs;
using FluentValidation;

namespace FitNote.Application.Validators;

public class LoginInputValidator : AbstractValidator<LoginInput> {
  public LoginInputValidator() {
    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Valid email is required")
      .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password is required")
      .MinimumLength(8).WithMessage("Password must be at least 8 characters long");
  }
}