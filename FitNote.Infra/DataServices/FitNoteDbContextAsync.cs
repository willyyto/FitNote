using FitNote.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitNote.Infra.DataServices;

public class FitNoteDbContextAsync : AsyncDbContext, IFitNoteDbContextAsync {
  public FitNoteDbContextAsync(DbContextOptions<FitNoteDbContextAsync> options) : base(options) { }

  public FitNoteDbContextAsync(DbContextOptions options) : base(options) { }

  // Enhanced DbSets with all new entities
  public DbSet<User> Users => Set<User>();

  protected override void OnModelCreating(ModelBuilder builder) {
    // Configure all entities with their respective configurations
    new UserConfig().Configure(builder.Entity<User>());

    base.OnModelCreating(builder);
  }
}