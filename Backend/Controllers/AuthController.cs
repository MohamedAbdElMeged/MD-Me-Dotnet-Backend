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
            await _registerValidator.ValidateAndThrowAsync(registerRequestDto);

            var user = await _authService.RegisterAsync(registerRequestDto);
            if (user is null)
            {
                return BadRequest("User can not be created");
            }
            return Ok(new{email = registerRequestDto.Email});
        }
    }
}
