namespace Backend.Dtos.Responses;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}

public class AuthResponseDto
{
    public UserResponseDto User { get; set; }
    public string Token { get; set; }
}
