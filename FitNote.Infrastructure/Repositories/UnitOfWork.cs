using FitNote.Core.Interfaces;
using FitNote.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitNote.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork {
  private readonly FitNoteDbContext _context;
  private readonly Dictionary<Type, object> _repositories = new();
  private IDbContextTransaction? _transaction;

  public UnitOfWork(FitNoteDbContext context) {
    _context = context;
  }

  public IWorkoutRepository Workouts =>
    (IWorkoutRepository)GetRepository<IWorkoutRepository, WorkoutRepository>();

  public IExerciseRepository Exercises =>
    (IExerciseRepository)GetRepository<IExerciseRepository, ExerciseRepository>();

  public IRepository<T> Repository<T>() where T : class {
    return (IRepository<T>)GetRepository<IRepository<T>, Repository<T>>();
  }

  private object GetRepository<TInterface, TImplementation>()
    where TImplementation : class {
    var type = typeof(TInterface);
    if (_repositories.TryGetValue(type, out var repository))
      return repository;

    repository = Activator.CreateInstance(typeof(TImplementation), _context)!;
    _repositories[type] = repository;
    return repository;
  }

  public async Task<int> SaveChangesAsync() {
    return await _context.SaveChangesAsync();
  }

  public async Task BeginTransactionAsync() {
    _transaction = await _context.Database.BeginTransactionAsync();
  }

  public async Task CommitTransactionAsync() {
    if (_transaction != null) {
      await _transaction.CommitAsync();
      await _transaction.DisposeAsync();
      _transaction = null;
    }
  }

  public async Task RollbackTransactionAsync() {
    if (_transaction != null) {
      await _transaction.RollbackAsync();
      await _transaction.DisposeAsync();
      _transaction = null;
    }
  }

  public void Dispose() {
    _transaction?.Dispose();
    _context.Dispose();
  }
}