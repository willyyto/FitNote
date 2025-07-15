namespace FitNote.Core.Interfaces;

public interface IUnitOfWork : IDisposable {
  IWorkoutRepository Workouts { get; }
  IExerciseRepository Exercises { get; }
  IRepository<T> Repository<T>() where T : class;
  Task<int> SaveChangesAsync();
  Task BeginTransactionAsync();
  Task CommitTransactionAsync();
  Task RollbackTransactionAsync();
}