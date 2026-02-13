using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Results;

namespace Backend.Services;

public interface INoteService
{
    public Task<Result<NoteResponseDto>> CreateNoteAsync(CreateNoteRequestDto createNoteRequestDto);
}