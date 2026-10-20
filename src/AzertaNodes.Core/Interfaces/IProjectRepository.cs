using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken);
        Task SaveProjectAsync(Project project, CancellationToken cancellationToken);
        Task SetProjectStateAsync(Guid projectId, bool isRunning, CancellationToken ct);
        Task EditProjectAsync(Project project, CancellationToken cancellationToken);
        Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken);
    }
}
