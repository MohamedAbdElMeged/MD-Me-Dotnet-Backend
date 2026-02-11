using System.Net.Mime;

namespace Backend.Dtos.Requests;

public class FileRequestDto
{
    public string FileName { get; set; }
    public string ParentId { get; set; }
    public string FileContent { get; set; }
    
}