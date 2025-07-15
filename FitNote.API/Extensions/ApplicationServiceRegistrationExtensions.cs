using FitNote.Application.Mappers;
using FitNote.Application.Services;
using FluentValidation;

namespace FitNote.Application.Extensions;

public static class ServiceCollectionExtensions {
  public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
    // Add AutoMapper
    services.AddAutoMapper(typeof(MappingProfile));

    // Add FluentValidation
    services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

    // Add MediatR
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

    // Add Application Services
    services.AddScoped<IWorkoutService, WorkoutService>();
    services.AddScoped<IExerciseService, ExerciseService>();
    services.AddScoped<IAuthService, AuthService>();

    return services;
  }
}