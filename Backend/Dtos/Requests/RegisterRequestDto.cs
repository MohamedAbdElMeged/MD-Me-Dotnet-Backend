using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.Requests;

public class RegisterRequestDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}