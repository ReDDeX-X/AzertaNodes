using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;

namespace AzertaNodes.Application.Implementations
{
    public class ContextService : IContextService
    {
        public Dictionary<string, NodeExecutionResult> GetInputs(GraphContext context, NodeDto nodeDto, ILookup<Guid, Edge> edgesByTarget)
        {
            var blackboard = context.Blackboard;
            var inputs = new Dictionary<string, NodeExecutionResult>(nodeDto.Settings?.Count ?? 0);

            if (nodeDto.Settings != null)
            {
                foreach (var setting in nodeDto.Settings)
                {
                    inputs[setting.Key] = NodeExecutionResult.Create(setting.Value);
                }
            }

            foreach (var edge in edgesByTarget[nodeDto.Id])
            {
                var executionResult = blackboard.TryGetValue((edge.SourceNodeId, edge.OutputKey), out var value) ? value : null;
                if (executionResult is null || !executionResult.IsSkipped)
                {
                    var saveValue = executionResult?.Value is not null ? NodeExecutionResult.Create(executionResult.Value) : NodeExecutionResult.CreateNullResult();
                    inputs[edge.InputKey] = saveValue;
                }
                else
                {
                    inputs[edge.InputKey] = NodeExecutionResult.Skipped();
                }
            }
            return inputs;
        }

        public void PropagateSkip(GraphContext context, Guid nodeId, ILookup<Guid, Edge> edgesBySource)
        {
            var blackboard = context.Blackboard;
            foreach (var edge in edgesBySource[nodeId])
            {
                blackboard[(nodeId, edge.OutputKey)] = NodeExecutionResult.Skipped();
            }
        }

        public void SaveOutputs(GraphContext context, Guid nodeId, string outputKey, NodeExecutionResult nodeExecutionResult)
        {
            if (nodeExecutionResult.Value is not null || nodeExecutionResult.IsSkipped) context.Blackboard[(nodeId, outputKey)] = nodeExecutionResult;
        }

        public void SaveFinalOutputs(GraphContext context, string outputKey, NodeExecutionResult nodeExecutionResult)
        {
            if (nodeExecutionResult.Value is not null) context.FinalOutputs[outputKey] = nodeExecutionResult;
        }
        public Dictionary<string, object> GetFinalOutputs(GraphContext context)
        {
            return context.FinalOutputs.ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
