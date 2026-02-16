namespace Backend.Dtos.Requests;

public class DeleteMultipleNotesRequestDto
{
    public List<Guid> Ids { get; set; }
}