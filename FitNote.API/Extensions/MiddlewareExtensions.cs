using FitNote.API.Middleware;

namespace FitNote.API.Extensions;

public static class MiddlewareExtensions {
  public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app) {
    return app.UseMiddleware<GlobalExceptionMiddleware>();
  }

  public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder app) {
    return app.UseMiddleware<PerformanceMiddleware>();
  }

  public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app) {
    return app.Use(async (context, next) => {
      // Add security headers before the response starts
      context.Response.OnStarting(() => {
        if (!context.Response.HasStarted) {
          var headers = context.Response.Headers;
          
          headers.TryAdd("X-Content-Type-Options", "nosniff");
          headers.TryAdd("X-Frame-Options", "DENY");
          headers.TryAdd("X-XSS-Protection", "1; mode=block");
          headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
          headers.TryAdd("X-Permitted-Cross-Domain-Policies", "none");
          headers.TryAdd("X-Robots-Tag", "noindex, nofollow");
          
          if (context.Request.IsHttps) {
            headers.TryAdd("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
          }
        }
        return Task.CompletedTask;
      });
      
      await next();
    });
  }
}