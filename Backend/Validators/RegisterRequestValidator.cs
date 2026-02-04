using System.Data;
using Backend.Data;
using Backend.Dtos.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Backend.Validators;

public class RegisterRequestValidator: AbstractValidator<RegisterRequestDto>
{
    private readonly AppDbContext _context;

    public RegisterRequestValidator(AppDbContext context)
    {
        _context = context;
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is Required")
            .EmailAddress().WithMessage("Invalid Email Format");
           //  .MustAsync(async (email, _) =>
           //      !await _context.Users.AnyAsync(x => x.Email == email)
           // ).WithMessage("Email is already Taken");


        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is Required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");

        RuleFor(x => x.PasswordConfirmation)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}