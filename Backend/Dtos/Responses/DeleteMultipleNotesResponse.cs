namespace Backend.Dtos.Responses;

public class DeleteMultipleNotesResponse
{
    public List<Guid> Deleted { get; set; }
    public List<Guid> Failed { get; set; }
}