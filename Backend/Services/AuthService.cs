#nullable enable
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Mappers;
using Backend.Results;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context,IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;

    }
    public async  Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        var user = await GetUserByEmail(registerRequestDto.Email, false);
        if (user != null)
        {
            return Result<AuthResponseDto>.Failure(new Error(
                Code: "USER_EXISTS",
                ErrorType: ErrorType.Conflict,
                Message: "User exists"
            ));
        }
        user = new User()
        {
            Email = registerRequestDto.Email,
        };
        var passwordHash = new PasswordHasher<User>().HashPassword(user,registerRequestDto.Password);
        user.PasswordHash = passwordHash;
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var token = CreateJwtToken(user);
        var authResponse = user.ToAuthResponseDto(token);
        return Result<AuthResponseDto>.Success(authResponse);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var user = await GetUserByEmail(email: loginRequestDto.Email, track: true);
        if (user == null ||
            new PasswordHasher<User>().
                VerifyHashedPassword(user, user.PasswordHash, loginRequestDto.Password) == PasswordVerificationResult.Failed)
        {
            return Result<AuthResponseDto>.Failure(new Error(
                Code: "INVALID_CREDENTIALS",
                ErrorType: ErrorType.Unauthorized,
                Message: "Invalid email or password"
            ));
        }
        var result = user.ToAuthResponseDto(CreateJwtToken(user));
        return Result<AuthResponseDto>.Success(result);
    }

    private async Task<User?> GetUserByEmail(string email, bool track)
    {
        var users = track ? _context.Users : _context.Users.AsNoTracking();

        return await users.FirstOrDefaultAsync(x => x.Email == email);
    }
    private string? CreateJwtToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Token")!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("Jwt:Issuer"),
            audience: _configuration.GetValue<string>("Jwt:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
    
}