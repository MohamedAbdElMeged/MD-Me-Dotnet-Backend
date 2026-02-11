using System.Security.Claims;
using Backend.Dtos.Requests;
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
                var validationFailure = Result<Backend.Dtos.Responses.UserResponseDto>.Failure(new Error(
                    Code: "VALIDATION_ERROR",
                    ErrorType: ErrorType.Validation,
                    Message: message
                ));
                return validationFailure.ToActionResult(this);
            }

            var result = await _authService.RegisterAsync(registerRequestDto);
            return result.ToActionResult(this);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _authService.LoginAsync(loginRequestDto);
            return result.ToActionResult(this);
        }
        
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            
            // TODO: implement get profile
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok($"authenticated {userId} ");
        }
    }
    
    
}
