
using FitNote.Core.Entities;
using FitNote.Core.Interfaces;
using FitNote.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitNote.Infrastructure.Repositories;

public class WorkoutRepository : Repository<Workout>, IWorkoutRepository
{
    public WorkoutRepository(FitNoteDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Workout>> GetUserWorkoutsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbSet.Where(w => w.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(w => w.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(w => w.Date <= endDate.Value);

        return await query
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Sets)
            .OrderByDescending(w => w.Date)
            .ToListAsync();
    }

    public async Task<Workout?> GetWorkoutWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(w => w.User)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Sets)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<Workout>> GetRecentWorkoutsAsync(Guid userId, int count = 10)
    {
        return await _dbSet
            .Where(w => w.UserId == userId)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Sets)
            .OrderByDescending(w => w.Date)
            .Take(count)
            .ToListAsync();
    }
}
