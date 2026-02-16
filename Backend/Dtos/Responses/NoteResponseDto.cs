namespace Backend.Dtos.Responses;

public class NoteResponseDto
{
    public Guid Id { get; set; }
    public string Path { get; set; }
    public string Title { get; set; }
    public Guid VaultId { get; set; }
}

public class NoteWithVaultDto : NoteResponseDto
{
    public VaultResponseDto Vault { get; set; }
}