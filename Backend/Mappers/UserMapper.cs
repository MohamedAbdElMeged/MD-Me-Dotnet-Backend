using Backend.Dtos.Responses;
using Backend.Entities;

namespace Backend.Mappers;

public static class UserMapper
{
    public static AuthResponseDto ToAuthResponseDto(this User user, string token)
    {
        return new AuthResponseDto()
        {
            User = user.ToUserResponseDto(),
            Token = token
        };
    }

    public static UserResponseDto ToUserResponseDto(this User user)
    {
        return new UserResponseDto()
        {
            Id = user.Id,
            Email = user.Email
        };
    }
}