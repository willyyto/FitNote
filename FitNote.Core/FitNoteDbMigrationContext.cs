using FitNote.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FitNote.Core;

public class FitNoteDbMigrationContext : DbContext {
  public FitNoteDbMigrationContext(DbContextOptions<FitNoteDbMigrationContext> opts) : base(opts) { }

  // DbSets for all entities - required for migrations
  public DbSet<User> Users => Set<User>();

  protected override void OnModelCreating(ModelBuilder builder) {
    Console.WriteLine("Configuring entity models for migration context...");

    // Apply all entity configurations
    new UserConfig().Configure(builder.Entity<User>());

    Console.WriteLine("Entity model configuration completed.");

    base.OnModelCreating(builder);
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    if (!optionsBuilder.IsConfigured) {
      // Fallback configuration if not configured externally
      var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false)
        .Build();

      var connectionString = configuration.GetConnectionString("FitNoteDbConnection");
      optionsBuilder.UseSqlServer(connectionString);
    }

    base.OnConfiguring(optionsBuilder);
  }
}