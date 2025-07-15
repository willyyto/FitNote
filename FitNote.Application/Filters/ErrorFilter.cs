using FluentValidation;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

namespace FitNote.Application.GraphQL.Filters;

public class ErrorFilter : IErrorFilter {
  public IError OnError(IError error) {
    return error.Exception switch {
      UnauthorizedAccessException => ErrorBuilder.New()
        .SetMessage("You are not authorized to perform this action.")
        .SetCode("UNAUTHORIZED")
        .Build(),

      ArgumentException => ErrorBuilder.New()
        .SetMessage(error.Exception.Message)
        .SetCode("INVALID_ARGUMENT")
        .Build(),

      KeyNotFoundException => ErrorBuilder.New()
        .SetMessage("The requested resource was not found.")
        .SetCode("NOT_FOUND")
        .Build(),

      ValidationException validationEx => ErrorBuilder.New()
        .SetMessage("Validation failed.")
        .SetCode("VALIDATION_ERROR")
        .SetExtension("errors",
          validationEx.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }))
        .Build(),

      _ => ErrorBuilder.New()
        .SetMessage("An unexpected error occurred.")
        .SetCode("INTERNAL_ERROR")
        .Build()
    };
  }
}