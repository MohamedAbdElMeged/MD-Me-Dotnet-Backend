using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Entities;
using Backend.Results;

namespace Backend.Services;

public interface INoteService
{
    public Task<Result<NoteResponseDto>> CreateNoteAsync(CreateNoteRequestDto createNoteRequestDto);
    public Task<Result<UploadNoteResponseDto>> CreatePresignedUrlForNoteAsync(Guid id, bool upload);
    public Task<Note?> GetNoteByIdASync(Guid id);
    Task<Result> DeleteNoteAsync(Guid id);
    Task<Result<DeleteMultipleNotesResponse>> DeleteMultipleNotesAsync(DeleteMultipleNotesRequestDto deleteMultipleNotesRequestDto);
}