using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;

namespace AzertaNodes.Infrastructure.Nodes
{
    public class HttpResponseNode : BaseNode
    {
        public override string Name { get; } = "HTTP response node";
        public override async ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return inputs;
        }
        public override Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs)
        {
            return Result<Dictionary<string, NodeExecutionResult>>.Success([]);
        }
    }
}
