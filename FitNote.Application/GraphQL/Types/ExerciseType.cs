using FitNote.Application.DTOs;
using FitNote.Core.Enums;

namespace FitNote.Application.GraphQL.Types;

public class ExerciseType : ObjectType<ExerciseDto>
{
    protected override void Configure(IObjectTypeDescriptor<ExerciseDto> descriptor)
    {
        descriptor.Field(e => e.Id).Type<NonNullType<IdType>>();
        descriptor.Field(e => e.Name).Type<NonNullType<StringType>>();
        descriptor.Field(e => e.Description).Type<StringType>();
        descriptor.Field(e => e.Category).Type<NonNullType<EnumType<ExerciseCategory>>>();
        descriptor.Field(e => e.PrimaryMuscleGroup).Type<NonNullType<EnumType<MuscleGroup>>>();
        descriptor.Field(e => e.SecondaryMuscleGroups).Type<ListType<EnumType<MuscleGroup>>>();
        descriptor.Field(e => e.Instructions).Type<StringType>();
        descriptor.Field(e => e.ImageUrl).Type<StringType>();
        descriptor.Field(e => e.IsDefault).Type<NonNullType<BooleanType>>();
        descriptor.Field(e => e.CreatedByUserId).Type<IdType>();
        descriptor.Field(e => e.CreatedByUser).Type<UserType>();
        descriptor.Field(e => e.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(e => e.UpdatedAt).Type<DateTimeType>();
    }
}
