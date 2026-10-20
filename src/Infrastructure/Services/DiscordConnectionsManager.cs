using AzertaNodes.Application.Interfaces;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AzertaNodes.Infrastructure.Services
{
    public class DiscordConnectionManager(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, ILogger<DiscordConnectionManager> logger) : IDiscordConnectionManager, IAsyncDisposable, IDisposable
    {
        private readonly ConcurrentDictionary<Guid, DiscordClient> clients = new();
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> locks = new();

        private bool disposed;

        public async Task<DiscordClient> GetOrCreateClientAsync(Guid projectId, Guid credentialId, CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (clients.TryGetValue(projectId, out var existingClient))
                return existingClient;

            var projectLock = locks.GetOrAdd(projectId, _ => new SemaphoreSlim(1, 1));
            await projectLock.WaitAsync(cancellationToken);

            try
            {
                if (clients.TryGetValue(projectId, out var client))
                    return client;

                var token = await GetDiscordTokenAsync(credentialId, cancellationToken);

                var config = new DiscordConfiguration
                {
                    Token = token,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
                    LoggerFactory = loggerFactory
                };

                var newClient = new DiscordClient(config);

                newClient.MessageCreated += OnMessageCreated;

                async Task OnMessageCreated(DiscordClient s, DSharpPlus.EventArgs.MessageCreateEventArgs eventArgs)
                {
                    using var eventScope = serviceProvider.CreateScope();
                    try
                    {
                        var scopedGraphService = eventScope.ServiceProvider.GetRequiredService<IGraphService>();
                        await scopedGraphService.ProcessAndRunAsync(projectId, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing message for project {ProjectId}", projectId);
                    }
                }

                try
                {
                    await newClient.ConnectAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to connect Discord client for project {ProjectId}", projectId);
                    newClient.MessageCreated -= OnMessageCreated;
                    newClient.Dispose();
                    throw;
                }

                clients[projectId] = newClient;
                return newClient;
            }
            finally
            {
                projectLock.Release();
            }
        }

        public async Task DisconnectClientAsync(Guid projectId)
        {
            if (clients.TryRemove(projectId, out var client))
            {
                try
                {
                    await client.DisconnectAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error disconnecting Discord client for project {ProjectId}", projectId);
                }
                finally
                {
                    client.Dispose();
                }
            }

            if (locks.TryRemove(projectId, out var projectLock))
            {
                projectLock.Dispose();
            }
        }
        private async Task<string> GetDiscordTokenAsync(Guid credentialId, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var credentialService = scope.ServiceProvider.GetRequiredService<ICredentialService>();
            var tokenResult = await credentialService.GetDecryptedCredentialAsync(credentialId, cancellationToken);

            if (tokenResult.IsFailure)
                throw new MissingDiscordTokenException();

            return tokenResult.GetValueOrThrow();
        }

        public async ValueTask DisposeAsync()
        {
            if (disposed)
            {
                GC.SuppressFinalize(this);
                return;
            }
            disposed = true;

            var tasks = clients.Values.Select(async client =>
            {
                try
                {
                    await client.DisconnectAsync();
                }
                finally
                {
                    client.Dispose();
                }
            });

            await Task.WhenAll(tasks);
            clients.Clear();

            foreach (var lockObj in locks.Values)
            {
                lockObj.Dispose();
            }
            locks.Clear();

            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    public class MissingDiscordTokenException : Exception
    {
        public MissingDiscordTokenException() : base("Discord bot token is missing or invalid.")
        {
        }
    }
}