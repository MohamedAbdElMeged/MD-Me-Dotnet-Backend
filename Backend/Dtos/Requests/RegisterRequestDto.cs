using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos.Requests;

public class RegisterRequestDto
{
    [Required]
    public string Email { get; set; }
    [StringLength(16,MinimumLength = 8,ErrorMessage = "Password must be between 8 and 16 characters")]

    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}