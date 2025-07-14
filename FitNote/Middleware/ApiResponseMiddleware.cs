using System.Net;
using System.Text.Json;
using FitNote.Core.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitNote.Middleware;

/// <summary>
///   Middleware to handle unhandled exceptions and format them as API responses
/// </summary>
public class ApiResponseMiddleware {
  private readonly ILogger<ApiResponseMiddleware> _logger;
  private readonly RequestDelegate _next;

  public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger) {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context) {
    try {
      await _next(context);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "An unhandled exception occurred");
      await HandleExceptionAsync(context, ex);
    }
  }

  private static async Task HandleExceptionAsync(HttpContext context, Exception exception) {
    var response = context.Response;
    response.ContentType = "application/json";

    var apiResponse = exception switch {
      UnauthorizedAccessException => new ApiResponseDto<object>(
        false, null, "Unauthorized access", null, (int)HttpStatusCode.Unauthorized),

      ArgumentException => new ApiResponseDto<object>(
        false, null, exception.Message, null, (int)HttpStatusCode.BadRequest),

      KeyNotFoundException => new ApiResponseDto<object>(
        false, null, "Resource not found", null, (int)HttpStatusCode.NotFound),

      InvalidOperationException => new ApiResponseDto<object>(
        false, null, exception.Message, null, (int)HttpStatusCode.BadRequest),

      _ => new ApiResponseDto<object>(
        false, null, "An internal server error occurred", new List<string> { exception.Message },
        (int)HttpStatusCode.InternalServerError)
    };

    response.StatusCode = apiResponse.StatusCode ?? (int)HttpStatusCode.InternalServerError;

    var jsonOptions = new JsonSerializerOptions {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      WriteIndented = true
    };

    var jsonResponse = JsonSerializer.Serialize(apiResponse, jsonOptions);
    await response.WriteAsync(jsonResponse);
  }
}