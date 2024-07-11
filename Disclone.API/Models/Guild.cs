namespace Disclone.API.Models;

public class Guild
{
    public Guid GuildId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public Guid? OwnerId { get; set; }
    
    public virtual User Owner { get; set; }
    public virtual ICollection<Subscription> Users { get; set; } = [];
}