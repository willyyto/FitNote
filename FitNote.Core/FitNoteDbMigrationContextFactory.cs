using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FitNote.Core;

public class FitNoteDbMigrationContextFactory : IDesignTimeDbContextFactory<FitNoteDbMigrationContext> {
  // Holds migration infrastructure settings
  private const string AppSettingsFilePath = "appsettings.json";

  public FitNoteDbMigrationContext CreateDbContext(string[] args) {
    Console.WriteLine("Created db context");
    return new FitNoteDbMigrationContext(GetDbContextOptions());
  }

  public static DbContextOptions<FitNoteDbMigrationContext> GetDbContextOptions() {
    Console.WriteLine("Starting migrations...");

    var configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile(AppSettingsFilePath)
      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
      .Build();

    var connectionString = configuration.GetConnectionString("FitNoteDbConnection");

    Console.WriteLine($"Attempting to run migrations with connection: '{connectionString}'");

    var dbContextBuilder =
      new DbContextOptionsBuilder<FitNoteDbMigrationContext>().UseSqlServer(connectionString);

    Console.WriteLine("Created db context options");

    return dbContextBuilder.Options;
  }
}