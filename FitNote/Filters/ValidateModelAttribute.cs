using FitNote.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FitNote.Filters;

/// <summary>
///   Action filter to validate model state and return consistent error responses
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute {
  public override void OnActionExecuting(ActionExecutingContext context) {
    if (!context.ModelState.IsValid) {
      var errors = context.ModelState
        .Where(x => x.Value?.Errors.Count > 0)
        .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
        .ToList();

      var response = new ApiResponseDto<object>(
        false,
        null,
        "Validation failed",
        errors,
        400
      );

      context.Result = new BadRequestObjectResult(response);
    }

    base.OnActionExecuting(context);
  }
}