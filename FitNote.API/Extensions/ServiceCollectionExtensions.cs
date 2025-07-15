using System.Text;
using FitNote.Application.GraphQL.Filters;
using FitNote.Application.GraphQL.Mutations;
using FitNote.Application.GraphQL.Queries;
using FitNote.Application.GraphQL.Types;
using FitNote.Core.Entities;
using FitNote.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FitNote.API.Extensions;

public static class ServiceCollectionExtensions {
  public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration) {
    // Add Identity
    services.AddIdentity<User, IdentityRole<Guid>>(options => {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // Set to true in production
      })
      .AddEntityFrameworkStores<FitNoteDbContext>()
      .AddDefaultTokenProviders();

    // Add JWT Authentication
    var jwtSettings = configuration.GetSection("Jwt");
    var secretKey = jwtSettings["Secret"];

    if (string.IsNullOrEmpty(secretKey)) throw new InvalidOperationException("JWT Secret is not configured");

    services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options => {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
          ClockSkew = TimeSpan.Zero,
          RoleClaimType = "role"
        };

        // Add GraphQL support for JWT
        options.Events = new JwtBearerEvents {
          OnMessageReceived = context => {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/graphql")) context.Token = accessToken;

            return Task.CompletedTask;
          }
        };
      });

    // Add Authorization
    services.AddAuthorization(options => {
      options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin", "Premium"));
      options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
      options.AddPolicy("RequirePremiumRole", policy => policy.RequireRole("Premium", "Admin"));
    });

    // Add GraphQL
    services
      .AddGraphQLServer()
      .AddQueryType<Query>()
      .AddMutationType<Mutation>()
      .AddType<UserType>()
      .AddType<WorkoutType>()
      .AddType<ExerciseType>()
      .AddType<WorkoutExerciseType>()
      .AddType<ExerciseSetType>()
      .AddAuthorization()
      .AddFiltering()
      .AddSorting()
      .AddProjections()
      .AddErrorFilter<ErrorFilter>()
      .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true); // Remove in production

    // Add CORS
    services.AddCors(options => {
      options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
      });

      // More restrictive CORS policy for production
      options.AddPolicy("Production", policy => {
        policy.WithOrigins("https://yourfrontend.com") // Replace with actual frontend URL
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
      });
    });

    return services;
  }
}