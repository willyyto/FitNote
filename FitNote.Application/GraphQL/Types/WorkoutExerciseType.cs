using FitNote.Application.DTOs;

namespace FitNote.Application.GraphQL.Types;
public class WorkoutExerciseType : ObjectType<WorkoutExerciseDto>

{
    protected override void Configure(IObjectTypeDescriptor<WorkoutExerciseDto> descriptor)
    {
        descriptor.Field(we => we.Id).Type<NonNullType<IdType>>();
        descriptor.Field(we => we.Order).Type<NonNullType<IntType>>();
        descriptor.Field(we => we.Notes).Type<StringType>();
        descriptor.Field(we => we.WorkoutId).Type<NonNullType<IdType>>();
        descriptor.Field(we => we.ExerciseId).Type<NonNullType<IdType>>();
        descriptor.Field(we => we.Exercise).Type<ExerciseType>();
        descriptor.Field(we => we.Sets).Type<ListType<ExerciseSetType>>();
        descriptor.Field(we => we.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(we => we.UpdatedAt).Type<DateTimeType>();
    }
}
