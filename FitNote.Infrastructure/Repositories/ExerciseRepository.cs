using FitNote.Core.Entities;
using FitNote.Core.Enums;
using FitNote.Core.Interfaces;
using FitNote.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitNote.Infrastructure.Repositories;

public class ExerciseRepository : Repository<Exercise>, IExerciseRepository
{
    public ExerciseRepository(FitNoteDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Exercise>> GetExercisesByCategoryAsync(ExerciseCategory category)
    {
        return await _dbSet
            .Where(e => e.Category == category)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exercise>> GetExercisesByMuscleGroupAsync(MuscleGroup muscleGroup)
    {
        return await _dbSet
            .Where(e => e.PrimaryMuscleGroup == muscleGroup || 
                       e.SecondaryMuscleGroups.Contains(muscleGroup))
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exercise>> GetUserExercisesAsync(Guid userId)
    {
        return await _dbSet
            .Where(e => e.CreatedByUserId == userId)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exercise>> GetDefaultExercisesAsync()
    {
        return await _dbSet
            .Where(e => e.IsDefault)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exercise>> SearchExercisesAsync(string searchTerm)
    {
        return await _dbSet
            .Where(e => e.Name.Contains(searchTerm) || 
                       (e.Description != null && e.Description.Contains(searchTerm)))
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
}
