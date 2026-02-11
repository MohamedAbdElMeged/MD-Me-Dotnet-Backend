using System.Security.Claims;
using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Extensions;
using Backend.Results;
using Backend.Services;
using Backend.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VaultsController(ICurrentUserService currentUser, IVaultService vaultService, IValidator<CreateVaultRequestDto> validator)
    : ControllerBase
{
    [Authorize]
    [HttpGet("my-vaults")]
    public async Task<IActionResult> GetUserVaults()
    {
        
        var result = await vaultService.GetUserVaults(currentUser.UserId);
        return result.ToActionResult(this);
    }


    [Authorize]
    [HttpPost("create-vault")]
    public async Task<IActionResult> CreateVault([FromBody] CreateVaultRequestDto createVaultRequestDto)
    {
        var validationResult = await validator.ValidateAsync(createVaultRequestDto);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            var validationFailure = Result<Backend.Dtos.Responses.VaultResponseDto>.Failure(new Error(
                Code: "VALIDATION_ERROR",
                ErrorType: ErrorType.Validation,
                Message: message
            ));
            return validationFailure.ToActionResult(this);
        }

        var result = await vaultService.CreateVaultAsync(createVaultRequestDto);
        return result.ToActionResult(this);

    }
    
    
}

