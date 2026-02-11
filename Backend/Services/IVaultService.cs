using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Results;

namespace Backend.Services;

public interface IVaultService
{
    public Task<Result<List<VaultResponseDto>>> GetUserVaults(Guid userId);
    Task<Result<VaultResponseDto>> CreateVaultAsync(CreateVaultRequestDto createVaultRequestDto);
}