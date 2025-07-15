using FitNote.Core.Entities;
using FitNote.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitNote.Infrastructure.Data;

public static class DbInitializer {
  public static async Task InitializeAsync(FitNoteDbContext context, UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager) {
    try {
      // Ensure database is created
      await context.Database.MigrateAsync();

      // Create roles
      await CreateRolesAsync(roleManager);

      // Seed default exercises
      await SeedDefaultExercisesAsync(context);

      // Create test user
      await CreateTestUserAsync(userManager);

      Console.WriteLine("Database initialization completed successfully.");
    }
    catch (Exception ex) {
      Console.WriteLine($"Database initialization failed: {ex.Message}");
      throw;
    }
  }

  private static async Task CreateRolesAsync(RoleManager<IdentityRole<Guid>> roleManager) {
    string[] roles = { "Admin", "User", "Premium" };

    foreach (var role in roles)
      if (!await roleManager.RoleExistsAsync(role)) {
        var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role) { Id = Guid.NewGuid() });
        if (!result.Succeeded)
          throw new InvalidOperationException(
            $"Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
      }
  }

  private static async Task SeedDefaultExercisesAsync(FitNoteDbContext context) {
    if (!await context.Exercises.AnyAsync()) {
      var exercises = new List<Exercise> {
        new() {
          Id = Guid.NewGuid(),
          Name = "Push-up",
          Description = "Standard push-up exercise targeting chest, triceps, and shoulders",
          Category = ExerciseCategory.BodyWeight,
          PrimaryMuscleGroup = MuscleGroup.Chest,
          SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Triceps, MuscleGroup.Shoulders },
          IsDefault = true,
          Instructions =
            "Start in plank position with hands slightly wider than shoulders. Lower your body until chest nearly touches the floor, then push back up to starting position.",
          CreatedAt = DateTime.UtcNow
        },
        new() {
          Id = Guid.NewGuid(),
          Name = "Bodyweight Squat",
          Description = "Basic bodyweight squat for lower body strength",
          Category = ExerciseCategory.BodyWeight,
          PrimaryMuscleGroup = MuscleGroup.Quadriceps,
          SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Glutes, MuscleGroup.Calves },
          IsDefault = true,
          Instructions =
            "Stand with feet shoulder-width apart. Lower body as if sitting back into a chair, keeping chest up and knees behind toes. Return to standing position.",
          CreatedAt = DateTime.UtcNow
        },
        new() {
          Id = Guid.NewGuid(),
          Name = "Bench Press",
          Description = "Barbell bench press for upper body strength",
          Category = ExerciseCategory.Strength,
          PrimaryMuscleGroup = MuscleGroup.Chest,
          SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Triceps, MuscleGroup.Shoulders },
          IsDefault = true,
          Instructions =
            "Lie on bench with eyes under the bar. Grip bar slightly wider than shoulders. Lower bar to chest with control, then press up to starting position.",
          CreatedAt = DateTime.UtcNow
        },
        new() {
          Id = Guid.NewGuid(),
          Name = "Deadlift",
          Description = "Conventional deadlift for posterior chain development",
          Category = ExerciseCategory.Strength,
          PrimaryMuscleGroup = MuscleGroup.Hamstrings,
          SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Glutes, MuscleGroup.Back },
          IsDefault = true,
          Instructions =
            "Stand with feet hip-width apart, bar over mid-foot. Bend at hips and knees to grip bar. Stand up by driving through heels and extending hips and knees simultaneously.",
          CreatedAt = DateTime.UtcNow
        },
        new() {
          Id = Guid.NewGuid(),
          Name = "Pull-up",
          Description = "Bodyweight pull-up for upper body pulling strength",
          Category = ExerciseCategory.BodyWeight,
          PrimaryMuscleGroup = MuscleGroup.Back,
          SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Biceps, MuscleGroup.Forearms },
          IsDefault = true,
          Instructions =
            "Hang from bar with palms facing away. Pull body up until chin clears the bar, then lower with control to starting position.",
          CreatedAt = DateTime.UtcNow
        },
        new() {
          Id = Guid.NewGuid(),
          Name = "Plank",
          Description = "Core stability exercise",
          Category = ExerciseCategory.BodyWeight,
          PrimaryMuscleGroup = MuscleGroup.Core,
          SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Shoulders },
          IsDefault = true,
          Instructions =
            "Start in push-up position but rest on forearms. Keep body in straight line from head to heels. Hold position while breathing normally.",
          CreatedAt = DateTime.UtcNow
        }
      };

      context.Exercises.AddRange(exercises);
      await context.SaveChangesAsync();
    }
  }

  private static async Task CreateTestUserAsync(UserManager<User> userManager) {
    var testEmail = "test@fitnote.com";
    var testUser = await userManager.FindByEmailAsync(testEmail);

    if (testUser == null) {
      testUser = new User {
        Id = Guid.NewGuid(),
        UserName = testEmail,
        Email = testEmail,
        FirstName = "Test",
        LastName = "User",
        EmailConfirmed = true,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
      };

      var result = await userManager.CreateAsync(testUser, "TestPassword123!");
      if (result.Succeeded)
        await userManager.AddToRoleAsync(testUser, "User");
      else
        throw new InvalidOperationException(
          $"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }
  }
}