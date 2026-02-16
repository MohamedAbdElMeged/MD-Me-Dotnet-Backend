using System.Security.Claims;
using Backend.Data;
using Backend.Entities;

namespace Backend.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext) : ICurrentUserService
{


    public Guid UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            return userId is null ? Guid.Empty : Guid.Parse(userId);
        }
    }
    public string? Email =>
        httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.Email)?
            .Value;

    public User? User =>
        dbContext.Users.FirstOrDefault(u => u.Id == UserId);
}