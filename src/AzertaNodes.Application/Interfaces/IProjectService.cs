using AzertaNodes.Application.DTOs;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using System.Text.Json;

namespace AzertaNodes.Application.Interfaces
{
    public interface IProjectService
    {
        public Task<Result<Project>> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<Result<IEnumerable<Project>>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken);
        public Task<Result<Project>> CreateProjectAsync(Project project, CancellationToken cancellationToken);
        public Task<Result<Project>> EditProjectAsync(Project project, CancellationToken cancellationToken, JsonElement? graphDefinition = null);
        public Task<Result<bool>> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken);
    }
}
