using FitNote.Core.Interfaces;
using FitNote.Infrastructure.Data;
using FitNote.Infrastructure.Repositories;
using FitNote.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FitNote.Infrastructure.Extensions;

public static class ServiceCollectionExtensions {
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
    IConfiguration configuration) {
    // Add DbContext
    services.AddDbContext<FitNoteDbContext>(options =>
      options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // Add Repositories
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IWorkoutRepository, WorkoutRepository>();
    services.AddScoped<IExerciseRepository, ExerciseRepository>();
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    // Add Services
    services.AddScoped<ITokenService, TokenService>();

    return services;
  }
}