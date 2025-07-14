using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace FitNote.Core;

public class IdGenerator : ValueGenerator<Guid> {
  public override bool GeneratesTemporaryValues => false;

  public override Guid Next(EntityEntry entry) {
    return Guid.NewGuid();
  }
}