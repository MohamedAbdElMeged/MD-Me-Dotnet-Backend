using Backend.Dtos.Requests;
using Backend.Extensions;
using Backend.Results;
using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterRequestDto> _registerValidator;

        public AuthController(IAuthService authService, IValidator<RegisterRequestDto> registerValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
        }
        
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerRequestDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                var validationFailure = Result<Backend.Dtos.Responses.UserResponse>.Failure(new Error(
                    ErrorType.Validation,
                    message,
                    "VALIDATION_ERROR"
                ));

                return validationFailure.ToActionResult(this);
            }
            
            var result = await _authService.RegisterAsync(registerRequestDto);
            return result.ToActionResult(this);
        }
    }
}
