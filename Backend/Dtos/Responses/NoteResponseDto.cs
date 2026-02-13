namespace Backend.Dtos.Responses;

public class NoteResponseDto
{
    public Guid Id { get; set; }
    public string Path { get; set; }
    public string Title { get; set; }
    public VaultResponseDto Vault { get; set; }
}