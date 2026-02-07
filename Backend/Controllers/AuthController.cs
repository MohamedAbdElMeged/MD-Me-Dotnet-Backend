using Backend.Dtos.Requests;
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
        public async Task<ActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            
            var result = await _authService.RegisterAsync(registerRequestDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok(new{email = registerRequestDto.Email});
        }
    }
}
