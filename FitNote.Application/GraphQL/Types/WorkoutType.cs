using FitNote.Application.DTOs;
using FitNote.Core.Enums;

namespace FitNote.Application.GraphQL.Types;

public class WorkoutType : ObjectType<WorkoutDto> {
  protected override void Configure(IObjectTypeDescriptor<WorkoutDto> descriptor) {
    descriptor.Field(w => w.Id).Type<NonNullType<IdType>>();
    descriptor.Field(w => w.Name).Type<NonNullType<StringType>>();
    descriptor.Field(w => w.Notes).Type<StringType>();
    descriptor.Field(w => w.Date).Type<NonNullType<DateTimeType>>();
    descriptor.Field(w => w.Duration).Type<TimeSpanType>();
    descriptor.Field(w => w.Status).Type<NonNullType<EnumType<WorkoutStatus>>>();
    descriptor.Field(w => w.UserId).Type<NonNullType<IdType>>();
    descriptor.Field(w => w.User).Type<UserType>();
    descriptor.Field(w => w.WorkoutExercises).Type<ListType<WorkoutExerciseType>>();
    descriptor.Field(w => w.CreatedAt).Type<NonNullType<DateTimeType>>();
    descriptor.Field(w => w.UpdatedAt).Type<DateTimeType>();
  }
}