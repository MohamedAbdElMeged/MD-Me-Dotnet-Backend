using System;
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
    public async Task<Result<List<VaultWithNoteResponseDto>>> GetUserVaults(Guid userId)
    {
        var vaults = await context.Vaults.Where(v => v.UserId == userId).ToListAsync();
        var vaultsResponse = vaults.Select(v => v.ToVaultWithNoteResponseDto()).ToList();
        return Result<List<VaultWithNoteResponseDto>>.Success(vaultsResponse);
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

    public async Task<Result> DeleteAsync(Guid id)
    {
        var vault = await GetVaultByIdAsync(id);
        if (vault is null)
            return new Result(false, CommonErrors.NotFoundError("vault"));

        context.Vaults.Remove(vault);
        await context.SaveChangesAsync();
        return new Result(true, null);
    }

    public async Task<Result<VaultResponseDto>> UpdateVaultAsync(Guid id, UpdateVaultDto updateVaultDto)
    {
        var vault = await GetVaultByIdAsync(id);
        if (vault is null)
            return Result<VaultResponseDto>.Failure(CommonErrors.NotFoundError("vault"));
        updateVaultDto.Id = id;
        vault = updateVaultDto.ToVaultEntityFromUpdateVaultRequestDto();
        context.Vaults.Update(vault);
        await context.SaveChangesAsync();
        return Result<VaultResponseDto>.Success(vault.ToVaultResponseDto());

    }

    public async Task<Vault?> GetVaultByIdAsync(Guid id, bool withNotes = false)
    {
        return await context.Vaults.FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Result<VaultWithNoteResponseDto>> GetVaultAsync(Guid id)
    {
        var vault = await GetVaultByIdAsync(id,true);
        return vault is null ? Result<VaultWithNoteResponseDto>.Failure(CommonErrors.NotFoundError("vault")) : Result<VaultWithNoteResponseDto>.Success(vault.ToVaultWithNoteResponseDto());
    }
}