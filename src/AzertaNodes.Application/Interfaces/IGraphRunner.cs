using AzertaNodes.Application.DTOs;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Application.Interfaces
{
    public interface IGraphRunner
    {
        Task<Dictionary<string, object>> RunAsync(Project project, List<NodeDto> nodes, IEnumerable<Edge> edges, CancellationToken cancellationToken = default);
    }
}