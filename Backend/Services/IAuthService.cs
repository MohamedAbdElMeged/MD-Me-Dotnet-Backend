using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Results;

namespace Backend.Services;

public interface IAuthService
{
    Task<Result<UserResponse>> RegisterAsync(RegisterRequestDto registerRequestDto);
}