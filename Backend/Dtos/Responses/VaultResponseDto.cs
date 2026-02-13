namespace Backend.Dtos.Responses;

public class VaultResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
}


public class VaultWithNoteResponseDto : VaultResponseDto
{
    public List<NoteResponseDto> Notes { get; set; }
}
