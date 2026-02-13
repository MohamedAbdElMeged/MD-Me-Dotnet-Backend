using Backend.Dtos.Requests;
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

    public static Vault ToVaultEntityFromUpdateVaultRequestDto(this UpdateVaultDto updateVaultDto)
    {
        return new Vault()
        {
            Id = updateVaultDto.Id,
            Name = updateVaultDto.Name
        };
    }

    public static VaultWithNoteResponseDto ToVaultWithNoteResponseDto(this Vault vault)
    {
        return new VaultWithNoteResponseDto()
        {
            Id = vault.Id,
            Name = vault.Name,
            Notes = new List<NoteResponseDto>(vault.Notes.Select(n => n.ToNoteResponseDto()))
        };
    }
}