using FluentValidation;
using Microsoft.Extensions.Logging;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

namespace FitNote.Application.GraphQL.Filters;

public class ErrorFilter : IErrorFilter {
  private readonly ILogger<ErrorFilter> _logger;

  public ErrorFilter(ILogger<ErrorFilter> logger) {
    _logger = logger;
  }

  public IError OnError(IError error) {
    // Log the error with correlation ID
    var correlationId = Guid.NewGuid().ToString();
    _logger.LogError(error.Exception, "GraphQL error occurred. CorrelationId: {CorrelationId}, Message: {Message}", 
      correlationId, error.Message);

    return error.Exception switch {
      UnauthorizedAccessException => ErrorBuilder.New()
        .SetMessage("You are not authorized to perform this action.")
        .SetCode("UNAUTHORIZED")
        .SetExtension("correlationId", correlationId)
        .Build(),

      ArgumentException argEx => ErrorBuilder.New()
        .SetMessage(argEx.Message)
        .SetCode("INVALID_ARGUMENT")
        .SetExtension("correlationId", correlationId)
        .Build(),

      KeyNotFoundException => ErrorBuilder.New()
        .SetMessage("The requested resource was not found.")
        .SetCode("NOT_FOUND")
        .SetExtension("correlationId", correlationId)
        .Build(),

      InvalidOperationException invOpEx => ErrorBuilder.New()
        .SetMessage(invOpEx.Message)
        .SetCode("INVALID_OPERATION")
        .SetExtension("correlationId", correlationId)
        .Build(),

      ValidationException validationEx => ErrorBuilder.New()
        .SetMessage("Validation failed.")
        .SetCode("VALIDATION_ERROR")
        .SetExtension("correlationId", correlationId)
        .SetExtension("errors",
          validationEx.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }))
        .Build(),

      TimeoutException => ErrorBuilder.New()
        .SetMessage("The request timed out.")
        .SetCode("TIMEOUT")
        .SetExtension("correlationId", correlationId)
        .Build(),

      TaskCanceledException => ErrorBuilder.New()
        .SetMessage("The request was cancelled.")
        .SetCode("CANCELLED")
        .SetExtension("correlationId", correlationId)
        .Build(),

      _ => ErrorBuilder.New()
        .SetMessage("An unexpected error occurred.")
        .SetCode("INTERNAL_ERROR")
        .SetExtension("correlationId", correlationId)
        .Build()
    };
  }
}