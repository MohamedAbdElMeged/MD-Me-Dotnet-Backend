using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Results;

namespace Backend.Services;

public interface INoteService
{
    public Task<Result<NoteResponseDto>> CreateNoteAsync(CreateNoteRequestDto createNoteRequestDto);
    public Task<Result<UploadNoteResponseDto>> CreatePresignedUrlForNoteAsync(Guid id);
    public Task<Note?> GetNoteByIdASync(Guid id);
}