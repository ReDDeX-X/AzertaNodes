using AzertaNodes.Application.DTOs;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;

namespace AzertaNodes.Application.Interfaces
{
    public interface IContextService
    {
        Dictionary<string, NodeExecutionResult> GetInputs(GraphContext context, NodeDto nodeDto, ILookup<Guid, Edge> edgesByTarget);
        void SaveOutputs(GraphContext context, Guid nodeId, string outputKey, NodeExecutionResult nodeExecutionResult);
        void PropagateSkip(GraphContext context, Guid nodeId, ILookup<Guid, Edge> edgesBySource);
        Dictionary<string, object> GetFinalOutputs(GraphContext context);
        void SaveFinalOutputs(GraphContext context, string outputKey, NodeExecutionResult nodeExecutionResult);
    }
}
