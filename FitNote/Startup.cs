using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FitNote.Core.Dtos;
using FitNote.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FitNote;

public class Startup {
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration) {
    _configuration = configuration;
  }

  /// <summary>
  ///   Add and configure services for the container
  /// </summary>
  /// <param name="services"></param>
  public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env) {
    services.AddControllers(options => { options.Filters.Add<ValidateModelAttribute>(); });

    services.AddControllers();

    services.Configure<RouteOptions>(options => {
      options.LowercaseUrls = true;
      options.LowercaseQueryStrings = true;
    });

    services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = _configuration["Jwt:Issuer"],
          ValidAudience = _configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Token"]))
        };
      });


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(options => {
      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Please enter token in this format: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
      });

      options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
          new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
              Type = ReferenceType.SecurityScheme,
              Id = "Bearer"
            }
          },
          new string[] { }
        }
      });
    });

    services.AddCors(options => {
      options.AddPolicy("AllowLocalhost",
        builder => {
          builder.WithOrigins("http://localhost:3000").WithOrigins("http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });
  }

  /// <summary>
  ///   Configure the Autofac container
  /// </summary>
  public void ConfigureHostContainer(ConfigureHostBuilder hostBuilder, IConfiguration config) {
    hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    hostBuilder.ConfigureContainer<ContainerBuilder>(containerBuilder => {
      containerBuilder
        .AddDatabaseSettings(config)
        .AddEfCoreDbContexts()
        .AddApplicationServices();
    });
  }

  /// <summary>
  ///   Configure the webapplication depending on the environment
  /// </summary>
  public void Configure(WebApplication app, IWebHostEnvironment env) {
    if (env.IsDevelopment())
      app.UseSwagger()
        .UseSwaggerUI()
        .UseDeveloperExceptionPage();

    else
      // Enable the exception handler route
      app.UseExceptionHandler("/error")
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        .UseHsts()
        .UseHttpsRedirection();

    app.UseCors("AllowLocalhost");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
  }
}

// Extension for Swagger configuration
public static class SwaggerExtensions {
  public static void ConfigureForApiResponseDto(this SwaggerGenOptions options) {
    // Add custom schema for ApiResponseDto
    options.MapType<ApiResponseDto<object>>(() => new OpenApiSchema {
      Type = "object",
      Properties = new Dictionary<string, OpenApiSchema> {
        ["success"] = new() { Type = "boolean" },
        ["data"] = new() { Type = "object" },
        ["message"] = new() { Type = "string", Nullable = true },
        ["errors"] = new() {
          Type = "array",
          Items = new OpenApiSchema { Type = "string" },
          Nullable = true
        },
        ["statusCode"] = new() { Type = "integer", Nullable = true }
      }
    });
  }
}