using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Results;

namespace Backend.Services;

public interface IVaultService
{
    public Task<Result<List<VaultWithNoteResponseDto>>> GetUserVaults(Guid userId);
    Task<Result<VaultResponseDto>> CreateVaultAsync(CreateVaultRequestDto createVaultRequestDto);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<VaultResponseDto>> UpdateVaultAsync(Guid id, UpdateVaultDto updateVaultDto);
    public Task<Vault?> GetVaultByIdAsync(Guid id,bool withNotes = false);
    Task<Result<VaultWithNoteResponseDto>> GetVaultAsync(Guid id);
}