using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Results;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserResponse>>> GetAllAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email
            })
            .ToListAsync();

        return Result<List<UserResponse>>.Success(users);
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return Result<UserResponse>.Failure(new Error(
                ErrorType.NotFound,
                "User not found.",
                "USER_NOT_FOUND"
            ));
        }

        return Result<UserResponse>.Success(new UserResponse
        {
            Id = user.Id,
            Email = user.Email
        });
    }

    public async Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return Result<UserResponse>.Failure(new Error(
                ErrorType.NotFound,
                "User not found.",
                "USER_NOT_FOUND"
            ));
        }

        var emailExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == request.Email && u.Id != id);

        if (emailExists)
        {
            return Result<UserResponse>.Failure(new Error(
                ErrorType.Conflict,
                "Email is already in use.",
                "EMAIL_ALREADY_EXISTS"
            ));
        }

        user.Email = request.Email;
        await _context.SaveChangesAsync();

        return Result<UserResponse>.Success(new UserResponse
        {
            Id = user.Id,
            Email = user.Email
        });
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return Result.Failure(new Error(
                ErrorType.NotFound,
                "User not found.",
                "USER_NOT_FOUND"
            ));
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
