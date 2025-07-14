using FitNote.Application.DTOs;

namespace FitNote.Application.GraphQL.Types;

public class UserType : ObjectType<UserDto> {
  protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor) {
    descriptor.Field(u => u.Id).Type<NonNullType<IdType>>();
    descriptor.Field(u => u.UserName).Type<NonNullType<StringType>>();
    descriptor.Field(u => u.Email).Type<NonNullType<StringType>>();
    descriptor.Field(u => u.FirstName).Type<NonNullType<StringType>>();
    descriptor.Field(u => u.LastName).Type<NonNullType<StringType>>();
    descriptor.Field(u => u.ProfilePictureUrl).Type<StringType>();
    descriptor.Field(u => u.CreatedAt).Type<NonNullType<DateTimeType>>();
    descriptor.Field(u => u.LastLoginAt).Type<DateTimeType>();
    descriptor.Field(u => u.IsActive).Type<NonNullType<BooleanType>>();
  }
}