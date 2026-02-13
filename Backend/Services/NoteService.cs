using Backend.Data;
using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Mappers;
using Backend.Results;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class NoteService(AppDbContext context, ICurrentUserService currentUser,
    IVaultService vaultService, IAwsService awsService) : INoteService
{


   
    public async Task<Result<NoteResponseDto>> CreateNoteAsync(CreateNoteRequestDto createNoteRequestDto)
    {
        var vault = await vaultService.GetVaultByIdAsync(createNoteRequestDto.VaultId);
        if (vault == null || vault.UserId != currentUser.UserId)
        {
            return Result<NoteResponseDto>.Failure(CommonErrors.NotFoundError("vault"));
        }
        var normalizedPathResult = GetNormalizedNotePath(vault.Name, createNoteRequestDto.Path, createNoteRequestDto.Title);
        if (!normalizedPathResult.IsSuccess)
        {
            return Result<NoteResponseDto>.Failure(normalizedPathResult.Error);
        }

        var normalizedPath = normalizedPathResult.Value;
        createNoteRequestDto.Path = normalizedPath;
        
        var note = await GetNote(createNoteRequestDto.VaultId, createNoteRequestDto.Path,
            createNoteRequestDto.Title);

        if (note != null) return Result<NoteResponseDto>.Success(note.ToNoteResponseDto());
        note = createNoteRequestDto.ToNoteEntityFromCreateDto();
        note.Vault = vault; 
        await context.Notes.AddAsync(note);
        await context.SaveChangesAsync();

        return  Result<NoteResponseDto>.Success(note.ToNoteResponseDto());
    }

    public async Task<Result<UploadNoteResponseDto>> CreatePresignedUrlForNoteAsync(Guid id)
    {
        var note = await GetNoteByIdASync(id);
        if (note == null)
            return  Result<UploadNoteResponseDto>.Failure(CommonErrors.NotFoundError("note"));

        var presignedUrl = await  awsService.GenerateUploadPresignedUrl(note.Path);
        var response = new UploadNoteResponseDto()
        {
            id = id,
            PresignedUrl = presignedUrl
        };
        return Result<UploadNoteResponseDto>.Success(response);

    }

    public async Task<Note> GetNoteByIdASync(Guid id)
    {
        return await context.Notes.FirstOrDefaultAsync(n => n.Id == id);
    }


    private async Task<Note?> GetNote(Guid vaultId, string path, string title)
    {
        return await context.Notes.FirstOrDefaultAsync(n => n.VaultId == vaultId && n.Path == path && n.Title == title);
    }
    
    private Result<string> GetNormalizedNotePath(string vaultName, string path, string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return Result<string>.Failure(new Error(
                Code: "INVALID_TITLE",
                ErrorType: ErrorType.Validation,
                Message: "Title cannot be empty"
            ));
        }

        if (title.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            title = title[..^3]; 
        }
    
        var normalizedKeyResult = NormalizeKey($"{vaultName}/{path}/{title}.md");
        return normalizedKeyResult;
    }

    
    private static Result<string> NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result<string>.Failure(new Error(
                Code: "INVALID_KEY",
                ErrorType: ErrorType.Validation,
                Message: "Key cannot be null or whitespace"
            ));
        }

        var normalized = key.Trim().Replace('\\', '/');
        while (normalized.StartsWith('/'))
        {
            normalized = normalized.Substring(1);
        }

        if (normalized.Contains(".."))
        {
            return Result<string>.Failure(new Error(
                Code: "INVALID_KEY",
                ErrorType: ErrorType.Validation,
                Message: "Key cannot contain '..'"
            ));
        }

        return Result<string>.Success(normalized);
    }
}