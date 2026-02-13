using Backend.Entities;

namespace Backend.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Email { get; }
    User? User { get; }
}