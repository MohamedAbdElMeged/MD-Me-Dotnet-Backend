#nullable enable
using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Results;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IValidator<RegisterRequestDto> _validator;

    public AuthService(AppDbContext context,IValidator<RegisterRequestDto> validator)
    {
        _context = context;
        _validator = validator;
    }
    public async  Task<Result<UserResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        var validationResult = await _validator.ValidateAsync(registerRequestDto);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage));
            Error error = new Error("Validation Error", ErrorType.VALIDATION_FAILED,errorMessage);
            return Result<UserResponseDto>.Failure(error);
        }
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