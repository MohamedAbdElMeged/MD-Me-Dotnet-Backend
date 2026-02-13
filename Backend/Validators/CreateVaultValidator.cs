using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Validators;

public class CreateVaultValidator : AbstractValidator<CreateVaultRequestDto>
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateVaultValidator(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
        RuleFor(x => x.Name).NotEmpty().WithMessage("Vault Name is Required");
        RuleFor(x => x.Name)
            .MustAsync(async (name, _) =>
                !await _context.Vaults.AnyAsync(x => x.Name == name && x.UserId == _currentUser.UserId )
            ).WithMessage("Vault Name must be unique per user");
    }
}