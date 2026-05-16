namespace Messaging.Messages.ProxyServerEvents.Models;

public record ProxyServerCredentials
{
    public string Username { get; init; } = null!;

    public string Password { get; init; } = null!;
}