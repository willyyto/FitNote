using FitNote.Core.Entities;

namespace FitNote.Core.Interfaces;

public interface IWorkoutRepository : IRepository<Workout>
{
    Task<IEnumerable<Workout>> GetUserWorkoutsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<Workout?> GetWorkoutWithDetailsAsync(Guid id);
    Task<IEnumerable<Workout>> GetRecentWorkoutsAsync(Guid userId, int count = 10);
}
