using Backend.Dtos.Responses;
using Backend.Entities;

namespace Backend.Mappers;

public static class VaultMapper
{
    public static VaultResponseDto ToVaultResponseDto(this Vault vault)
    {
        return new VaultResponseDto()
        {
            Id = vault.Id,
            Name = vault.Name
        };
    }
}