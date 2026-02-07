using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class User: BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string RefreshToken { get; set; }
}

