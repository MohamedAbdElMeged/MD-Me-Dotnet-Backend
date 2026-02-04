namespace Backend.Dtos.Requests;

public class RegisterRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}