using AzertaNodes.Application.DTOs;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Application.Interfaces
{
    public interface IGraphValidationService
    {
        public Result<bool> ValidateGraph(Project project, IEnumerable<NodeDto> nodes, IEnumerable<Edge> edges);
    }
}