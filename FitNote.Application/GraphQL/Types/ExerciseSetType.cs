using FitNote.Application.DTOs;
using FitNote.Core.Enums;

namespace FitNote.Application.GraphQL.Types;

public class ExerciseSetType : ObjectType<ExerciseSetDto>
{
    protected override void Configure(IObjectTypeDescriptor<ExerciseSetDto> descriptor)
    {
        descriptor.Field(s => s.Id).Type<NonNullType<IdType>>();
        descriptor.Field(s => s.SetNumber).Type<NonNullType<IntType>>();
        descriptor.Field(s => s.Reps).Type<IntType>();
        descriptor.Field(s => s.Weight).Type<DecimalType>();
        descriptor.Field(s => s.Duration).Type<TimeSpanType>();
        descriptor.Field(s => s.Distance).Type<DecimalType>();
        descriptor.Field(s => s.Type).Type<NonNullType<EnumType<SetType>>>();
        descriptor.Field(s => s.IsCompleted).Type<NonNullType<BooleanType>>();
        descriptor.Field(s => s.Notes).Type<StringType>();
        descriptor.Field(s => s.WorkoutExerciseId).Type<NonNullType<IdType>>();
        descriptor.Field(s => s.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(s => s.UpdatedAt).Type<DateTimeType>();
    }
}
