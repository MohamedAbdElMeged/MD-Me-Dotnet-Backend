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

    public async Task<Result<VaultNotesTreeResponseDto>> GetVaultNotesTreeAsync(
        Guid vaultId,
        string? path,
        bool recursive,
        int page,
        int pageSize)
    {
        var vault = await context.Vaults
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == vaultId && v.UserId == currentUser.UserId);

        if (vault is null)
            return Result<VaultNotesTreeResponseDto>.Failure(CommonErrors.NotFoundError("vault"));

        var prefixResult = NormalizeVaultPath(vault.Name, path);
        if (!prefixResult.IsSuccess)
            return Result<VaultNotesTreeResponseDto>.Failure(prefixResult.Error);

        var prefix = prefixResult.Value;
        var basePath = path?.Trim('/') ?? string.Empty;

        var notes = await context.Notes
            .AsNoTracking()
            .Where(n => n.VaultId == vault.Id && n.Path.StartsWith(prefix))
            .OrderBy(n => n.Path)
            .ToListAsync();

        var response = new VaultNotesTreeResponseDto
        {
            Path = basePath,
            Recursive = recursive,
            Page = page,
            PageSize = pageSize
        };

        if (recursive)
        {
            response.TotalNotes = notes.Count;
            response.Nodes = BuildFullTree(prefix, basePath, notes);
            return Result<VaultNotesTreeResponseDto>.Success(response);
        }

        response.Nodes = BuildOneLevel(prefix, basePath, notes, page, pageSize, out var totalNotes);
        response.TotalNotes = totalNotes;
        return Result<VaultNotesTreeResponseDto>.Success(response);
    }

    private List<TreeNodeDto> BuildOneLevel(
        string prefix,
        string basePath,
        List<Note> notes,
        int page,
        int pageSize,
        out int totalNotes)
    {
        var folders = new Dictionary<string, TreeNodeDto>(StringComparer.OrdinalIgnoreCase);
        var directNotes = new List<TreeNodeDto>();

        foreach (var note in notes)
        {
            var remainder = note.Path.Substring(prefix.Length);
            var slashIndex = remainder.IndexOf('/');

            if (slashIndex >= 0)
            {
                var folderName = remainder.Substring(0, slashIndex);
                if (string.IsNullOrWhiteSpace(folderName)) continue;

                if (!folders.TryGetValue(folderName, out var folder))
                {
                    var folderPath = string.IsNullOrWhiteSpace(basePath)
                        ? folderName
                        : $"{basePath.TrimEnd('/')}/{folderName}";

                    folder = new TreeNodeDto
                    {
                        Type = "folder",
                        Name = folderName,
                        Path = folderPath,
                        NoteCount = 0
                    };
                    folders[folderName] = folder;
                }

                folder.NoteCount = (folder.NoteCount ?? 0) + 1;
                continue;
            }

            directNotes.Add(new TreeNodeDto
            {
                Type = "note",
                Name = note.Title,
                Path = basePath,
                Note = note.ToNoteResponseDto()
            });
        }

        totalNotes = directNotes.Count;

        var pagedNotes = directNotes
            .OrderBy(n => n.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var folderNodes = folders.Values
            .OrderBy(n => n.Name)
            .ToList();

        var nodes = new List<TreeNodeDto>(folderNodes.Count + pagedNotes.Count);
        nodes.AddRange(folderNodes);
        nodes.AddRange(pagedNotes);
        return nodes;
    }

    private List<TreeNodeDto> BuildFullTree(string prefix, string basePath, List<Note> notes)
    {
        var root = new TreeNodeDto
        {
            Type = "folder",
            Name = "",
            Path = basePath,
            Children = new List<TreeNodeDto>(),
            NoteCount = 0
        };

        foreach (var note in notes)
        {
            var remainder = note.Path.Substring(prefix.Length);
            var segments = remainder.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0) continue;
            
            var cursor = root;

            for (var i = 0; i < segments.Length - 1; i++)
            {
                cursor.Children ??= new List<TreeNodeDto>();
                var segment = segments[i];

                var child = cursor.Children.FirstOrDefault(c =>
                    c.Type == "folder" && c.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));

                if (child == null)
                {
                    var folderPath = string.IsNullOrWhiteSpace(cursor.Path)
                        ? segment
                        : $"{cursor.Path.TrimEnd('/')}/{segment}";

                    child = new TreeNodeDto
                    {
                        Type = "folder",
                        Name = segment,
                        Path = folderPath,
                        Children = new List<TreeNodeDto>(),
                        NoteCount = 0
                    };
                    cursor.Children.Add(child);
                }

                child.NoteCount = (child.NoteCount ?? 0) + 1;
                cursor = child;
            }

            cursor.Children ??= new List<TreeNodeDto>();
            cursor.Children.Add(new TreeNodeDto
            {
                Type = "note",
                Name = note.Title,
                Path = cursor.Path,
                Note = note.ToNoteResponseDto()
            });
        }

        SortTree(root);
        return root.Children ?? new List<TreeNodeDto>();
    }

    private void SortTree(TreeNodeDto node)
    {
        if (node.Children == null) return;

        node.Children = node.Children
            .OrderBy(n => n.Type)
            .ThenBy(n => n.Name)
            .ToList();

        foreach (var child in node.Children)
        {
            if (child.Type == "folder")
                SortTree(child);
        }
    }

    private Result<string> NormalizeVaultPath(string vaultName, string? path)
    {
        var raw = string.IsNullOrWhiteSpace(path)
            ? $"{currentUser.UserId}/{vaultName}/"
            : $"{currentUser.UserId}/{vaultName}/{path.Trim('/')}/";

        var normalized = raw.Trim().Replace('\\', '/');
        while (normalized.StartsWith('/'))
        {
            normalized = normalized.Substring(1);
        }

        if (normalized.Contains(".."))
        {
            return Result<string>.Failure(new Error(
                Code: "INVALID_PATH",
                ErrorType: ErrorType.Validation,
                Message: "Path cannot contain '..'"
            ));
        }

        return Result<string>.Success(normalized);
    }
}
