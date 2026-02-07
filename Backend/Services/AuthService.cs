#nullable enable
using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<UserResponse>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == registerRequestDto.Email);

        if (userExists)
        {
            return Result<UserResponse>.Failure(new Error(
                ErrorType.Conflict,
                "Email is already in use.",
                "EMAIL_ALREADY_EXISTS"
            ));
        }

        var user = new User()
        {
            Email = registerRequestDto.Email,
        };
        var passwordHash = new PasswordHasher<User>().HashPassword(user,registerRequestDto.Password);
        user.PasswordHash = passwordHash;
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var userResponse = new UserResponse()
        {
            Id = user.Id,
            Email = user.Email
        };
        return Result<UserResponse>.Success(userResponse);


    }
}