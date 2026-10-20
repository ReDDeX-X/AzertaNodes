using DSharpPlus;

namespace AzertaNodes.Application.Interfaces
{
    public interface IDiscordConnectionManager
    {
        Task<DiscordClient> GetOrCreateClientAsync(Guid projectId, Guid credentialId, CancellationToken cancellationToken);
        Task DisconnectClientAsync(Guid projectId);
    }
}
