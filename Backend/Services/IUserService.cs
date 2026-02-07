using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Results;

namespace Backend.Services;

public interface IUserService
{
    Task<Result<List<UserResponse>>> GetAllAsync();
    Task<Result<UserResponse>> GetByIdAsync(Guid id);
    Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequestDto request);
    Task<Result> DeleteAsync(Guid id);
}
