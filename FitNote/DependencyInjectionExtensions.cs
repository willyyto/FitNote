using Autofac;
using FitNote.Core;
using FitNote.Core.Entities;
using FitNote.Infra;
using FitNote.Infra.DataServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FitNote;

public static class EfContextRegistrationExtensions {
    /// <summary>
    ///   Add the application layer services
    /// </summary>
    /// <param name="containerBuilder"></param>
    /// <returns></returns>
    public static ContainerBuilder AddApplicationServices(this ContainerBuilder containerBuilder) {
    containerBuilder
      .RegisterType<CancellationTokenAccessor>()
      .As<ICancellationTokenAccessor>()
      .InstancePerLifetimeScope();

    containerBuilder.RegisterAttributeTaggedServices<InstanceScopedServiceAttribute>();
    containerBuilder.RegisterAttributeTaggedServices<InstanceScopedBusinessServiceAttribute>();
    containerBuilder.RegisterType<PasswordHasher<User>>().As<IPasswordHasher<User>>().SingleInstance();


    return containerBuilder;
  }

    /// <summary>
    ///   Register any services tagged with the instance registration attribute
    /// </summary>
    /// <param name="assembly">The assembly to search (passing the tag's assembly is an easy start)</param>
    /// <seealso cref="InstanceScopedServiceAttribute" />
    private static ContainerBuilder RegisterAttributeTaggedServices<T>(this ContainerBuilder containerBuilder)
    where T : Attribute {
    containerBuilder.RegisterAssemblyTypes(typeof(T).Assembly)
      .Where(type => type.GetCustomAttributes(typeof(T), false).Any())
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    return containerBuilder;
  }

    /// <summary>
    ///   Add the relevant EF Core db contexts
    /// </summary>
    public static ContainerBuilder AddEfCoreDbContexts(this ContainerBuilder builder) {
    return builder
      .AddFitNoteDbContext()
      .AddManagementMigrationsDbContext();
  }

    /// <summary>
    ///   Configure the ef core database (sets the db connection string)
    /// </summary>
    public static ContainerBuilder AddDatabaseSettings(this ContainerBuilder containerBuilder, IConfiguration config) {
    var databaseSettings = new DatabaseSettings(
      config.GetConnectionString("FitNoteDbConnection")
    );

    containerBuilder.RegisterInstance(databaseSettings).AsSelf().SingleInstance();

    return containerBuilder;
  }

  private static ContainerBuilder AddDbContextOptions<TContext>(this ContainerBuilder containerBuilder)
    where TContext : DbContext {
    containerBuilder.Register(sp => {
        var loggerFactory = sp.Resolve<ILoggerFactory>();
        var dbSettings = sp.Resolve<DatabaseSettings>();
        return new DbContextOptionsBuilder<TContext>()
          .UseLoggerFactory(loggerFactory)
          .UseSqlServer(dbSettings.ConnectionString)
          .EnableDetailedErrors()
          .EnableSensitiveDataLogging()
          .Options; // make sure to return options here! Otherwise we'll register the builder
      })
      .AsSelf()
      .SingleInstance();

    return containerBuilder;
  }

  private static ContainerBuilder AddFitNoteDbContext(this ContainerBuilder builder) {
    builder
      .AddDbContextOptions<FitNoteDbContextAsync>()
      .RegisterType<FitNoteDbContextAsync>()
      .As<IFitNoteDbContextAsync>()
      .InstancePerLifetimeScope();

    return builder;
  }

  private static ContainerBuilder AddManagementMigrationsDbContext(this ContainerBuilder builder) {
    builder
      .RegisterType<FitNoteDbMigrationContext>()
      .WithParameter("opts", FitNoteDbMigrationContextFactory.GetDbContextOptions())
      .InstancePerLifetimeScope();

    return builder;
  }
}