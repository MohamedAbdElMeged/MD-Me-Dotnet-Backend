using System.Data;
using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Services;
using FluentValidation;

namespace Backend.Validators;

public class CreateNoteValidator : AbstractValidator<CreateNoteRequestDto>
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateNoteValidator(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
        RuleFor(x => x.VaultId).NotEmpty().WithMessage("Vault Id is Required");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is Required");
        RuleFor(x => x.Path).NotEmpty().WithMessage("Path is Required");
        RuleFor(x => x.Path)
            .MustAsync(async (path, _) => !path.Contains("../")).WithMessage("Path cannot contain ../");
    }
    
}