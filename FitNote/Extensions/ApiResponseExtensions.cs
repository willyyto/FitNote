using FitNote.Core.Dtos;

namespace FitNote.Extensions;

public static class ApiResponseExtensions {
    /// <summary>
    ///   Creates a successful API response
    /// </summary>
    public static ApiResponseDto<T> ToSuccessResponse<T>(this T data, string? message = null) {
    return new ApiResponseDto<T>(
      true,
      data,
      message ?? "Operation completed successfully",
      null,
      200
    );
  }

    /// <summary>
    ///   Creates an error API response from a string message
    /// </summary>
    public static ApiResponseDto<T> ToErrorResponse<T>(this string message, List<string>? errors = null,
    int statusCode = 400) {
    return new ApiResponseDto<T>(
      false,
      default,
      message,
      errors,
      statusCode
    );
  }

    /// <summary>
    ///   Creates an error API response from an exception
    /// </summary>
    public static ApiResponseDto<T> ToErrorResponse<T>(this Exception exception, int statusCode = 500) {
    var errors = new List<string> { exception.Message };

    // Add inner exception messages if they exist
    var innerException = exception.InnerException;
    while (innerException != null) {
      errors.Add(innerException.Message);
      innerException = innerException.InnerException;
    }

    return new ApiResponseDto<T>(
      false,
      default,
      "An error occurred while processing the request",
      errors,
      statusCode
    );
  }

    /// <summary>
    ///   Creates an error API response from validation errors
    /// </summary>
    public static ApiResponseDto<T> ToValidationErrorResponse<T>(this List<ValidationErrorDto> validationErrors) {
    var errorMessages = validationErrors.Select(e => $"{e.Field}: {e.Message}").ToList();

    return new ApiResponseDto<T>(
      false,
      default,
      "Validation failed",
      errorMessages,
      400
    );
  }

    /// <summary>
    ///   Creates a not found error response
    /// </summary>
    public static ApiResponseDto<T> ToNotFoundResponse<T>(string? message = null) {
    return new ApiResponseDto<T>(
      false,
      default,
      message ?? "Resource not found",
      null,
      404
    );
  }

    /// <summary>
    ///   Creates an unauthorized error response
    /// </summary>
    public static ApiResponseDto<T> ToUnauthorizedResponse<T>(string? message = null) {
    return new ApiResponseDto<T>(
      false,
      default,
      message ?? "Unauthorized access",
      null,
      401
    );
  }

    /// <summary>
    ///   Creates a forbidden error response
    /// </summary>
    public static ApiResponseDto<T> ToForbiddenResponse<T>(string? message = null) {
    return new ApiResponseDto<T>(
      false,
      default,
      message ?? "Access forbidden",
      null,
      403
    );
  }

    /// <summary>
    ///   Creates a created response (201)
    /// </summary>
    public static ApiResponseDto<T> ToCreatedResponse<T>(this T data, string? message = null) {
    return new ApiResponseDto<T>(
      true,
      data,
      message ?? "Resource created successfully",
      null,
      201
    );
  }

    /// <summary>
    ///   Creates a no content response (204)
    /// </summary>
    public static ApiResponseDto<T> ToNoContentResponse<T>(string? message = null) {
    return new ApiResponseDto<T>(
      true,
      default,
      message ?? "Operation completed successfully",
      null,
      204
    );
  }
}