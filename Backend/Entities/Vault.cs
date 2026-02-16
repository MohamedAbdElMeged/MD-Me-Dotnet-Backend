namespace Backend.Entities;

public class Vault : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public string Name { get; set; }

    public List<Note> Notes { get; set; } = new List<Note>();
}