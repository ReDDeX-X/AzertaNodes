using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using AzertaNodes.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AzertaNodes.Infrastructure.Persistence
{
    public class ProjectRepository(AppDbContext dbContext) : IProjectRepository
    {
        public async Task<Project?> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await dbContext.ProjectSet.FindAsync([id], cancellationToken);
        }
        public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.ProjectSet
                .Where(p => p.UserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken)
        {
            return await dbContext.ProjectSet
                .Where(p => p.IsRunning)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        public async Task SaveProjectAsync(Project project, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            dbContext.ProjectSet.Add(project);
        }
        public async Task SetProjectStateAsync(Guid projectId, bool isRunning, CancellationToken ct)
        {
            await dbContext.ProjectSet
                .Where(p => p.Id == projectId)
                .ExecuteUpdateAsync(updates => updates.SetProperty(p => p.IsRunning, isRunning), ct);
        }
        public async Task EditProjectAsync(Project project, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            dbContext.ProjectSet.Update(project);
        }
        public async Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken)
        {
            await dbContext.ProjectSet
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
