using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;

namespace AzertaNodes.Infrastructure.Nodes
{
    public class TestConsoleWriteLineNode : BaseNode
    {
        public override string Name { get; } = "Test Console Write Line Node";
        public override async ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
        {
            var text = inputs["text"].Value;
            Console.WriteLine(text);
            Console.WriteLine("teeeest");
            return [];
        }
        public override Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs)
        {
            return Result<Dictionary<string, NodeExecutionResult>>.Success([]);
        }
    }
}
