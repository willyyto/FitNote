using FitNote.API.Middleware;

namespace FitNote.API.Extensions;

public static class MiddlewareExtensions {
  public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app) {
    return app.UseMiddleware<GlobalExceptionMiddleware>();
  }
}