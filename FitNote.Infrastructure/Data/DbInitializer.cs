using FitNote.Core.Entities;
using FitNote.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitNote.Infrastructure.Data;

public static class DbInitializer {
    public static async Task InitializeAsync(FitNoteDbContext context, UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager) {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Create roles
        await CreateRolesAsync(roleManager);

        // Seed default exercises
        await SeedDefaultExercisesAsync(context);

        // Create test user
        await CreateTestUserAsync(userManager);
    }

    private static async Task CreateRolesAsync(RoleManager<IdentityRole<Guid>> roleManager) {
        string[] roles = { "Admin", "User" };

        foreach (var role in roles) {
            if (!await roleManager.RoleExistsAsync(role)) {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private static async Task SeedDefaultExercisesAsync(FitNoteDbContext context) {
        if (!context.Exercises.Any()) {
            var exercises = new List<Exercise> {
                new Exercise {
                    Id = Guid.NewGuid(),
                    Name = "Push-up",
                    Description = "Standard push-up exercise",
                    Category = ExerciseCategory.BodyWeight,
                    PrimaryMuscleGroup = MuscleGroup.Chest,
                    SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Triceps, MuscleGroup.Shoulders },
                    IsDefault = true,
                    Instructions = "Start in plank position, lower body to ground, push back up",
                    CreatedAt = DateTime.UtcNow
                },
                new Exercise {
                    Id = Guid.NewGuid(),
                    Name = "Squat",
                    Description = "Bodyweight squat",
                    Category = ExerciseCategory.BodyWeight,
                    PrimaryMuscleGroup = MuscleGroup.Quadriceps,
                    SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Glutes, MuscleGroup.Calves },
                    IsDefault = true,
                    Instructions = "Stand with feet shoulder-width apart, lower body as if sitting back into chair",
                    CreatedAt = DateTime.UtcNow
                },
                new Exercise {
                    Id = Guid.NewGuid(),
                    Name = "Bench Press",
                    Description = "Barbell bench press",
                    Category = ExerciseCategory.Strength,
                    PrimaryMuscleGroup = MuscleGroup.Chest,
                    SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Triceps, MuscleGroup.Shoulders },
                    IsDefault = true,
                    Instructions = "Lie on bench, grip barbell, lower to chest, press up",
                    CreatedAt = DateTime.UtcNow
                },
                new Exercise {
                    Id = Guid.NewGuid(),
                    Name = "Deadlift",
                    Description = "Conventional deadlift",
                    Category = ExerciseCategory.Strength,
                    PrimaryMuscleGroup = MuscleGroup.Hamstrings,
                    SecondaryMuscleGroups = new List<MuscleGroup> { MuscleGroup.Glutes, MuscleGroup.Back },
                    IsDefault = true,
                    Instructions = "Stand with feet hip-width apart, bend at hips and knees, lift barbell",
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

            await userManager.CreateAsync(testUser, "TestPassword123!");
            await userManager.AddToRoleAsync(testUser, "User");
        }
    }
}
