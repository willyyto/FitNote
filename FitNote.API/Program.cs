using FitNote.API.Extensions;
using FitNote.Application.Extensions;
using FitNote.Core.Entities;
using FitNote.Infrastructure.Data;
using FitNote.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .WriteTo.File("logs/fitnote-.txt", rollingInterval: RollingInterval.Day)
  .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider.GetRequiredService<FitNoteDbContext>();
  var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    
  await DbInitializer.InitializeAsync(context, userManager, roleManager);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) {
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.Run();