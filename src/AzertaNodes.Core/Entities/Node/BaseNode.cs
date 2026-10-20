using AzertaNodes.Core.Interfaces;

namespace AzertaNodes.Core.Entities.Node
{
    public abstract class BaseNode : INode
    {
        public abstract string Name { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public required NodeMetaData MetaData { get; set; }
        public bool IsTrigger { get; protected set; } = false;
        public abstract ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken);
        public abstract Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs);
        protected static T? GetInput<T>(Dictionary<string, NodeExecutionResult> inputs, string key)
        {
            if (!inputs.TryGetValue(key, out var value))
            {
                return default;
            }
            return value.GetValue<T>();
        }
    }
}
