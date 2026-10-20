using AzertaNodes.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzertaNodes.Infrastructure.Services
{
    public class DiscordBootstrapService(IServiceScopeFactory scopeFactory, IDiscordConnectionManager connectionManager, ILogger<DiscordBootstrapService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Discord Bootstrap Service is starting...");

            using var scope = scopeFactory.CreateScope();

            var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();

            try
            {
                var activeProjects = await projectService.GetActiveProjectsAsync(stoppingToken);

                logger.LogInformation("Found {Count} active projects to connect.", activeProjects.Count());

                foreach (var project in activeProjects)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    try
                    {
                        if (!project.MainCredentialId.HasValue)
                        {
                            logger.LogWarning("Project {ProjectId} does not have a CredentialId. Skipping Discord connection.", project.Id);
                            continue;
                        }
                        await connectionManager.GetOrCreateClientAsync(project.Id, project.MainCredentialId.Value, stoppingToken);

                        logger.LogInformation("Successfully connected Discord for project {ProjectId}", project.Id);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to initialize Discord for project {ProjectId}", project.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Critical error during Discord bootstrapping");
            }
            await Task.CompletedTask;
        }
    }
}
