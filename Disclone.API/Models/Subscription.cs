namespace Disclone.API.Models;

public enum SubscriptionStatus
{
    Member,
    Banned
}

public class Subscription
{
    public Guid SubscriptionId { get; set; }
    public Guid? UserId { get; set; }
    public Guid GuildId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public string? Permissions { get; set; }

    public User User { get; set; }
    public Guild Guild { get; set; }
}