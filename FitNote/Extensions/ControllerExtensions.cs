using FitNote.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FitNote.Extensions;

public static class ControllerExtensions {
    /// <summary>
    ///   Returns an Ok result with ApiResponseDto wrapper
    /// </summary>
    public static IActionResult OkResponse<T>(this ControllerBase controller, T data, string? message = null) {
    return controller.Ok(data.ToSuccessResponse(message));
  }

    /// <summary>
    ///   Returns a Created result with ApiResponseDto wrapper
    /// </summary>
    public static IActionResult CreatedResponse<T>(this ControllerBase controller, string actionName, object routeValues,
    T data, string? message = null) {
    return controller.CreatedAtAction(actionName, routeValues, data.ToCreatedResponse(message));
  }

    /// <summary>
    ///   Returns a BadRequest result with ApiResponseDto wrapper
    /// </summary>
    public static IActionResult BadRequestResponse<T>(this ControllerBase controller, string message,
    List<string>? errors = null) {
    return controller.BadRequest(message.ToErrorResponse<T>(errors));
  }

    /// <summary>
    ///   Returns a NotFound result with ApiResponseDto wrapper
    /// </summary>
    public static IActionResult NotFoundResponse<T>(this ControllerBase controller, string? message = null) {
    return controller.NotFound(ApiResponseExtensions.ToNotFoundResponse<T>(message));
  }

    /// <summary>
    ///   Returns an Unauthorized result with ApiResponseDto wrapper
    /// </summary>
    public static IActionResult UnauthorizedResponse<T>(this ControllerBase controller, string? message = null) {
    return controller.Unauthorized(ApiResponseExtensions.ToUnauthorizedResponse<T>(message));
  }

    /// <summary>
    ///   Returns a Forbidden result with ApiResponseDto wrapper
    /// </summary>
    public static IActionResult ForbiddenResponse<T>(this ControllerBase controller, string? message = null) {
    return controller.StatusCode(403, ApiResponseExtensions.ToForbiddenResponse<T>(message));
  }

    /// <summary>
    ///   Returns a validation error response
    /// </summary>
    public static IActionResult ValidationErrorResponse<T>(this ControllerBase controller,
    List<ValidationErrorDto> validationErrors) {
    return controller.BadRequest(validationErrors.ToValidationErrorResponse<T>());
  }

    /// <summary>
    ///   Returns an exception error response
    /// </summary>
    public static IActionResult ExceptionResponse<T>(this ControllerBase controller, Exception exception,
    int statusCode = 500) {
    return controller.StatusCode(statusCode, exception.ToErrorResponse<T>(statusCode));
  }
}