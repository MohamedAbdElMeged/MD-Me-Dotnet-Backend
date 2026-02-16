namespace Backend.Entities;


public class Note : BaseEntity
{
    public Guid VaultId { get; set; }
    public virtual Vault Vault { get; set; }

    public string Path { get; set; }
    
    public string Title { get; set; }

    public virtual User User => Vault.User;
    public virtual Guid UserId => Vault.UserId;
}