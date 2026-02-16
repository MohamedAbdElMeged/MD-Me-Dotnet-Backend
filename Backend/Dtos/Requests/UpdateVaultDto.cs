using System.Text.Json.Serialization;

namespace Backend.Dtos.Requests;

public class UpdateVaultDto
{
    public string Name { get; set; }
    [JsonIgnore]
    public Guid Id { get; set; }
    
}