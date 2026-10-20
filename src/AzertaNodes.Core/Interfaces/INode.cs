using AzertaNodes.Core.Entities.Node;
using AzertaNodes.Core.Entities;

namespace AzertaNodes.Core.Interfaces
{
    public interface INode
    {
        Guid Id { get; }
        NodeMetaData MetaData { get; set; }
        ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken);
        Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs);
    }
}
