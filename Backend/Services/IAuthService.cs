using Backend.Dtos.Requests;
using Backend.Entities;

namespace Backend.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterRequestDto registerRequestDto);
}