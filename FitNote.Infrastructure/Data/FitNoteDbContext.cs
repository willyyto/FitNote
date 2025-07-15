using System.Text.Json;
using FitNote.Core.Entities;
using FitNote.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FitNote.Infrastructure.Data;

public class FitNoteDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid> {
  public FitNoteDbContext(DbContextOptions<FitNoteDbContext> options) : base(options) { }

  public DbSet<Exercise> Exercises { get; set; }
  public DbSet<Workout> Workouts { get; set; }
  public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
  public DbSet<ExerciseSet> ExerciseSets { get; set; }
  public DbSet<UserSubscription> UserSubscriptions { get; set; }

  protected override void OnModelCreating(ModelBuilder builder) {
    base.OnModelCreating(builder);

    // Configure Exercise entity
    builder.Entity<Exercise>(entity => {
      entity.ToTable("Exercises");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
      entity.Property(e => e.Description).HasMaxLength(500);
      entity.Property(e => e.Instructions).HasMaxLength(2000);
      entity.Property(e => e.Category).IsRequired().HasConversion<string>();
      entity.Property(e => e.PrimaryMuscleGroup).IsRequired().HasConversion<string>();
      entity.Property(e => e.CreatedAt).IsRequired();
      entity.Property(e => e.IsDefault).IsRequired();

      // Configure SecondaryMuscleGroups with proper value comparer
      var muscleGroupsComparer = new ValueComparer<ICollection<MuscleGroup>>(
        (c1, c2) => c1!.SequenceEqual(c2!),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToList());

      entity.Property(e => e.SecondaryMuscleGroups)
        .HasConversion(
          v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
          v => JsonSerializer.Deserialize<List<MuscleGroup>>(v, (JsonSerializerOptions?)null) ?? new List<MuscleGroup>()
        )
        .Metadata.SetValueComparer(muscleGroupsComparer);

      // Configure relationships
      entity.HasMany(e => e.WorkoutExercises)
        .WithOne(we => we.Exercise)
        .HasForeignKey(we => we.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(e => e.CreatedByUser)
        .WithMany(u => u.CreatedExercises)
        .HasForeignKey(e => e.CreatedByUserId)
        .OnDelete(DeleteBehavior.SetNull);

      // Configure indexes
      entity.HasIndex(e => e.Name);
      entity.HasIndex(e => e.Category);
      entity.HasIndex(e => e.PrimaryMuscleGroup);
      entity.HasIndex(e => e.IsDefault);
    });

    // Configure User entity
    builder.Entity<User>(entity => {
      entity.ToTable("Users");
      entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
      entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
      entity.Property(u => u.CreatedAt).IsRequired();
      entity.Property(u => u.IsActive).IsRequired();

      // Configure relationships
      entity.HasMany(u => u.Workouts)
        .WithOne(w => w.User)
        .HasForeignKey(w => w.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasMany(u => u.CreatedExercises)
        .WithOne(e => e.CreatedByUser)
        .HasForeignKey(e => e.CreatedByUserId)
        .OnDelete(DeleteBehavior.SetNull);
    });

    // Configure Workout entity
    builder.Entity<Workout>(entity => {
      entity.ToTable("Workouts");
      entity.HasKey(w => w.Id);
      entity.Property(w => w.Name).IsRequired().HasMaxLength(100);
      entity.Property(w => w.Notes).HasMaxLength(500); // Changed from Description to Notes
      entity.Property(w => w.Date).IsRequired();
      entity.Property(w => w.Status).IsRequired().HasConversion<string>();
      entity.Property(w => w.CreatedAt).IsRequired();

      // Configure relationships
      entity.HasMany(w => w.WorkoutExercises)
        .WithOne(we => we.Workout)
        .HasForeignKey(we => we.WorkoutId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(w => w.User)
        .WithMany(u => u.Workouts)
        .HasForeignKey(w => w.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      // Configure indexes
      entity.HasIndex(w => w.UserId);
      entity.HasIndex(w => w.Date);
      entity.HasIndex(w => w.Status);
    });

    // Configure WorkoutExercise entity
    builder.Entity<WorkoutExercise>(entity => {
      entity.ToTable("WorkoutExercises");
      entity.HasKey(we => we.Id);
      entity.Property(we => we.Order).IsRequired(); // Changed from OrderIndex to Order
      entity.Property(we => we.CreatedAt).IsRequired();

      // Configure relationships
      entity.HasMany(we => we.Sets)
        .WithOne(s => s.WorkoutExercise)
        .HasForeignKey(s => s.WorkoutExerciseId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(we => we.Workout)
        .WithMany(w => w.WorkoutExercises)
        .HasForeignKey(we => we.WorkoutId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(we => we.Exercise)
        .WithMany(e => e.WorkoutExercises)
        .HasForeignKey(we => we.ExerciseId)
        .OnDelete(DeleteBehavior.Cascade);

      // Configure indexes
      entity.HasIndex(we => we.WorkoutId);
      entity.HasIndex(we => we.ExerciseId);
    });

    // Configure ExerciseSet entity
    builder.Entity<ExerciseSet>(entity => {
      entity.ToTable("ExerciseSets");
      entity.HasKey(s => s.Id);
      entity.Property(s => s.SetNumber).IsRequired();
      entity.Property(s => s.Type).IsRequired().HasConversion<string>(); // Changed from SetType to Type
      entity.Property(s => s.Weight).HasPrecision(5, 2);
      entity.Property(s => s.Distance).HasPrecision(8, 2);
      entity.Property(s => s.Duration).HasColumnType("time");
      entity.Property(s => s.CreatedAt).IsRequired();

      // Configure relationships
      entity.HasOne(s => s.WorkoutExercise)
        .WithMany(we => we.Sets)
        .HasForeignKey(s => s.WorkoutExerciseId)
        .OnDelete(DeleteBehavior.Cascade);

      // Configure indexes
      entity.HasIndex(s => s.WorkoutExerciseId);
      entity.HasIndex(s => s.SetNumber);
    });

    // Configure UserSubscription entity
    builder.Entity<UserSubscription>(entity => {
      entity.ToTable("UserSubscriptions");
      entity.HasKey(us => us.Id);
      entity.Property(us => us.Tier).IsRequired().HasConversion<string>();
      entity.Property(us => us.StartDate).IsRequired();
      entity.Property(us => us.IsActive).IsRequired();
      entity.Property(us => us.CreatedAt).IsRequired();

      // Configure one-to-one relationship properly
      entity.HasOne(us => us.User)
        .WithOne(u => u.Subscription)
        .HasForeignKey<UserSubscription>(us => us.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      // Configure indexes
      entity.HasIndex(us => us.UserId);
      entity.HasIndex(us => us.IsActive);
    });

    // Configure Identity tables with custom names
    builder.Entity<IdentityRole<Guid>>(entity => { entity.ToTable("Roles"); });
    builder.Entity<IdentityUserRole<Guid>>(entity => { entity.ToTable("UserRoles"); });
    builder.Entity<IdentityUserClaim<Guid>>(entity => { entity.ToTable("UserClaims"); });
    builder.Entity<IdentityUserLogin<Guid>>(entity => { entity.ToTable("UserLogins"); });
    builder.Entity<IdentityRoleClaim<Guid>>(entity => { entity.ToTable("RoleClaims"); });
    builder.Entity<IdentityUserToken<Guid>>(entity => { entity.ToTable("UserTokens"); });
  }
}