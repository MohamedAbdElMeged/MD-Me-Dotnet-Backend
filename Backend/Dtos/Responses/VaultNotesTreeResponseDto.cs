namespace Backend.Dtos.Responses;

public class VaultNotesTreeResponseDto
{
    public string Path { get; set; } = "";
    public bool Recursive { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalNotes { get; set; }
    public List<TreeNodeDto> Nodes { get; set; } = new();
}

public class TreeNodeDto
{
    public string Type { get; set; } = ""; // folder | note
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public int? NoteCount { get; set; }
    public NoteResponseDto? Note { get; set; }
    public List<TreeNodeDto>? Children { get; set; }
}
