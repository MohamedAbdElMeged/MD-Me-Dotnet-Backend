namespace Backend.Dtos.Responses;

public class UploadNoteResponseDto
{
    public Guid id { get; set; }
    public string PresignedUrl { get; set; }
}