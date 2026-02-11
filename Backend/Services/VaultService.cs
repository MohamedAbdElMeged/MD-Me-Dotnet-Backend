using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Mappers;
using Backend.Results;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class VaultService(AppDbContext context, ICurrentUserService currentUser) : IVaultService
{
    // To-Do add pagination
    public async Task<Result<List<VaultResponseDto>>> GetUserVaults(Guid userId)
    {
        var vaults = await context.Vaults.Where(v => v.UserId == userId).ToListAsync();
        List<VaultResponseDto> vaultsResponse = new List<VaultResponseDto>();
        if (vaults.Count != 0)
        {
            vaultsResponse = vaults.Select(v => v.ToVaultResponseDto()).ToList();
        }
        
        return  Result<List<VaultResponseDto>>.Success(vaultsResponse);
    }

    public async Task<Result<VaultResponseDto>> CreateVaultAsync(CreateVaultRequestDto createVaultRequestDto)
    {
        var vault = new Vault()
        {
            Name = createVaultRequestDto.Name,
            UserId = currentUser.UserId
        };
        await context.Vaults.AddAsync(vault);
        await context.SaveChangesAsync();
        return Result<VaultResponseDto>.Success(vault.ToVaultResponseDto());
    }
}