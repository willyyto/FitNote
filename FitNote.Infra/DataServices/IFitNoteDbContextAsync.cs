using FitNote.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitNote.Infra.DataServices;

public interface IFitNoteDbContextAsync {
  DbSet<User> Users { get; }
  Task<int> SaveChangesAsync(CancellationToken ct);
}