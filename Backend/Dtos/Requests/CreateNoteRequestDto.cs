namespace Backend.Dtos.Requests;

public class CreateNoteRequestDto
{
    public Guid VaultId { get; set; }
    public string Path { get; set; }
    public string Title { get; set; }
}