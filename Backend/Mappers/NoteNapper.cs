using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;

namespace Backend.Mappers;

public static class NoteNapper
{
    public static Note ToNoteEntityFromCreateDto(this CreateNoteRequestDto note)
    {
        return new Note()
        {
            Path = note.Path,
            Title = note.Title,
            VaultId = note.VaultId
        };
    }

    public static NoteResponseDto ToNoteResponseDto(this Note note)
    {
        return new NoteResponseDto()
        {
            Id = note.Id,
            Path = note.Path,
            Title = note.Title,
            VaultId = note.VaultId
        };
    }

    public static NoteWithVaultDto ToNoteWithVaultDto(this Note note)
    {
        return new NoteWithVaultDto()
        {
            Id = note.Id,
            Path = note.Path,
            Title = note.Title,
            VaultId = note.VaultId,
            Vault = note.Vault.ToVaultResponseDto()
        };
    }
}