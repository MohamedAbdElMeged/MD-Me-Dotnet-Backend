#nullable enable
using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Entities;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }
    public async  Task<User?> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        var user = new User()
        {
            Email = registerRequestDto.Email,
        };
        var passwordHash = new PasswordHasher<User>().HashPassword(user,registerRequestDto.Password);
        user.PasswordHash = passwordHash;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
 

        
    }
}