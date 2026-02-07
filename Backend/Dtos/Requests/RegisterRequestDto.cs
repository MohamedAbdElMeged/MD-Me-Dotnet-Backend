using Microsoft.Build.Framework;

namespace Backend.Dtos.Requests;

public class RegisterRequestDto
{
    [Required]
    public string Email { get; set; }
    
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}