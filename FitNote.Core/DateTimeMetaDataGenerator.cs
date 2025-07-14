using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace FitNote.Core;

public class DateTimeMetaDataGenerator : ValueGenerator<DateTime> {
  public override bool GeneratesTemporaryValues => false;

  public override DateTime Next(EntityEntry entry) {
    return DateTime.UtcNow;
  }
}