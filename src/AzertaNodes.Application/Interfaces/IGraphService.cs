using AzertaNodes.Core.Entities;

namespace AzertaNodes.Application.Interfaces
{
    public interface IGraphService
    {
        Task<Result<Dictionary<string, object>>> ProcessAndRunAsync(Guid projectId, CancellationToken ct);
    }
}
