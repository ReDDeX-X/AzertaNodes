using System.Collections.Concurrent;
using AzertaNodes.Core.Entities.Node;

namespace AzertaNodes.Core.Entities
{
    public class GraphContext
    {
        public ConcurrentDictionary<(Guid NodeId, string outputKey), NodeExecutionResult> Blackboard { get; set; } = new();
        public ConcurrentDictionary<string, object> FinalOutputs { get; set; } = new();
    }
}
