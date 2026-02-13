using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Mappers;
using Backend.Results;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class NoteService(AppDbContext context, ICurrentUserService currentUser, IVaultService vaultService) : INoteService
{


   
    public async Task<Result<NoteResponseDto>> CreateNoteAsync(CreateNoteRequestDto createNoteRequestDto)
    {
        var vault = await vaultService.GetVaultByIdAsync(createNoteRequestDto.VaultId);
        if (vault == null || vault.UserId != currentUser.UserId)
        {
            return Result<NoteResponseDto>.Failure(CommonErrors.NotFoundError("vault"));
        }
        var note = await GetNote(createNoteRequestDto.VaultId, createNoteRequestDto.Path,
            createNoteRequestDto.Title);

        if (note != null) return Result<NoteResponseDto>.Success(note.ToNoteResponseDto());
        note = createNoteRequestDto.ToNoteEntityFromCreateDto();
        note.Vault = vault; 
        await context.Notes.AddAsync(note);
        await context.SaveChangesAsync();

        return  Result<NoteResponseDto>.Success(note.ToNoteResponseDto());
    }



    private async Task<Note?> GetNote(Guid vaultId, string path, string title)
    {
        return await context.Notes.FirstOrDefaultAsync(n => n.VaultId == vaultId && n.Path == path && n.Title == title);
    }
}