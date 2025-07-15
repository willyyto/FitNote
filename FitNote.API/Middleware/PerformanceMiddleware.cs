using System.Diagnostics;

namespace FitNote.API.Middleware;

public class PerformanceMiddleware {
  private readonly RequestDelegate _next;
  private readonly ILogger<PerformanceMiddleware> _logger;

  public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger) {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context) {
    var stopwatch = Stopwatch.StartNew();
    
    // Register callback to add header before response starts
    context.Response.OnStarting(() => {
      if (!context.Response.HasStarted && stopwatch.IsRunning) {
        context.Response.Headers.Add("X-Response-Time", $"{stopwatch.ElapsedMilliseconds}ms");
      }
      return Task.CompletedTask;
    });
    
    try {
      await _next(context);
    }
    finally {
      stopwatch.Stop();
      var elapsed = stopwatch.ElapsedMilliseconds;
      
      // Log slow requests
      if (elapsed > 1000) {
        _logger.LogWarning("Slow request detected: {Method} {Path} took {ElapsedMs}ms, Status: {StatusCode}, User: {User}",
          context.Request.Method,
          context.Request.Path,
          elapsed,
          context.Response.StatusCode,
          context.User?.Identity?.Name ?? "Anonymous");
      }
      else if (elapsed > 500) {
        _logger.LogInformation("Request took {ElapsedMs}ms: {Method} {Path}, Status: {StatusCode}",
          elapsed,
          context.Request.Method,
          context.Request.Path,
          context.Response.StatusCode);
      }
    }
  }
}