using Backend.Dtos.Requests;
using Backend.Dtos.Responses;
using Backend.Extensions;
using Backend.Results;
using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController(INoteService noteService, IValidator<CreateNoteRequestDto> createValidator) : ControllerBase
    {
        [Authorize]
        [HttpPost("")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> CreateNote(CreateNoteRequestDto createNoteRequestDto)
        {
            var validationResult = await createValidator.ValidateAsync(createNoteRequestDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                var validationFailure = Result<NoteResponseDto>.Failure(new Error(
                    Code: "VALIDATION_ERROR",
                    ErrorType: ErrorType.Validation,
                    Message: message
                ));
                return validationFailure.ToActionResult(this);
            }
            var result = await noteService.CreateNoteAsync(createNoteRequestDto);
            return result.ToActionResult(this);
        }
    }
}
