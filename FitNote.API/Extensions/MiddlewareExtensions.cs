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
      context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
      context.Response.Headers.Add("X-Frame-Options", "DENY");
      context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
      context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
      context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
      context.Response.Headers.Add("X-Robots-Tag", "noindex, nofollow");
      
      if (context.Request.IsHttps) {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
      }
      
      await next();
    });
  }
}