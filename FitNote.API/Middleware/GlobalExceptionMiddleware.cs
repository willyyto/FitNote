using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

namespace FitNote.API.Middleware;

public class GlobalExceptionMiddleware {
  private readonly IWebHostEnvironment _environment;
  private readonly ILogger<GlobalExceptionMiddleware> _logger;
  private readonly RequestDelegate _next;

  public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger,
    IWebHostEnvironment environment) {
    _next = next;
    _logger = logger;
    _environment = environment;
  }

  public async Task InvokeAsync(HttpContext context) {
    try {
      await _next(context);
    }
    catch (Exception ex) {
      _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
    context.Response.ContentType = "application/json";

    var response = new ProblemDetails();

    switch (exception) {
      case UnauthorizedAccessException:
        response.Status = (int)HttpStatusCode.Unauthorized;
        response.Title = "Unauthorized";
        response.Detail = "You are not authorized to access this resource";
        break;

      case ArgumentException:
        response.Status = (int)HttpStatusCode.BadRequest;
        response.Title = "Bad Request";
        response.Detail = exception.Message;
        break;

      case KeyNotFoundException:
        response.Status = (int)HttpStatusCode.NotFound;
        response.Title = "Not Found";
        response.Detail = "The requested resource was not found";
        break;

      case InvalidOperationException:
        response.Status = (int)HttpStatusCode.BadRequest;
        response.Title = "Invalid Operation";
        response.Detail = exception.Message;
        break;

      default:
        response.Status = (int)HttpStatusCode.InternalServerError;
        response.Title = "Internal Server Error";
        response.Detail = _environment.IsDevelopment()
          ? exception.ToString()
          : "An error occurred while processing your request";
        break;
    }

    context.Response.StatusCode = response.Status.Value;

    var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    await context.Response.WriteAsync(jsonResponse);
  }
}