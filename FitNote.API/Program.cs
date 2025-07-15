using FitNote.API.Extensions;
using FitNote.Application.Extensions;
using FitNote.Core.Entities;
using FitNote.Infrastructure.Data;
using FitNote.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
  .WriteTo.File("logs/fitnote-.txt", 
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 30,
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
  .CreateLogger();

builder.Host.UseSerilog();

// Add Rate Limiting
builder.Services.AddRateLimiter(options => {
  options.AddFixedWindowLimiter("GeneralPolicy", configure => {
    configure.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:GeneralLimit", 100);
    configure.Window = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("RateLimiting:WindowInMinutes", 1));
    configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    configure.QueueLimit = 10;
  });

  options.AddFixedWindowLimiter("AuthPolicy", configure => {
    configure.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:AuthLimit", 5);
    configure.Window = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("RateLimiting:WindowInMinutes", 1));
    configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    configure.QueueLimit = 2;
  });

  options.RejectionStatusCode = 429;
});

// Add services to the container
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Add response compression
builder.Services.AddResponseCompression(options => {
  options.EnableForHttps = true;
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope()) {
  var context = scope.ServiceProvider.GetRequiredService<FitNoteDbContext>();
  var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

  try {
    await DbInitializer.InitializeAsync(context, userManager, roleManager);
    Log.Information("Database initialization completed successfully");
  }
  catch (Exception ex) {
    Log.Fatal(ex, "Database initialization failed");
    throw;
  }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) {
  app.UseDeveloperExceptionPage();
  Log.Information("Development environment detected - enabling detailed error pages");
}
else {
  app.UseExceptionHandler("/Error");
  app.UseHsts();
  Log.Information("Production environment detected - using production error handling");
}

// Security and performance middleware
app.UseSecurityHeaders();
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UsePerformanceMonitoring();

// Apply rate limiting
app.UseRateLimiter();

// Apply CORS policy
if (app.Environment.IsDevelopment()) {
  app.UseCors("AllowAll");
  Log.Information("Development CORS policy applied");
}
else {
  app.UseCors("Production");
  Log.Information("Production CORS policy applied");
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Global exception handling (should be after auth for user context)
app.UseGlobalExceptionHandling();

// Map controllers
app.MapControllers();

// Map GraphQL with rate limiting
app.MapGraphQL()
   .RequireRateLimiting("GeneralPolicy");

try {
  Log.Information("Starting FitNote API on {Environment}", app.Environment.EnvironmentName);
  Log.Information("GraphQL endpoint available at: /graphql");
  
  if (app.Environment.IsDevelopment()) {
    Log.Information("GraphQL IDE available at: /graphql (Banana Cake Pop)");
    Log.Information("Test user credentials - Email: test@fitnote.com, Password: TestPassword123!");
  }
  
  app.Run();
}
catch (Exception ex) {
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally {
  Log.CloseAndFlush();
}
