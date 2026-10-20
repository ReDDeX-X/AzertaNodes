using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;
using DSharpPlus.Entities;

namespace AzertaNodes.Infrastructure.Nodes.DiscordNodes
{
    public class DiscordTriggerNode : BaseNode
    {
        public override string Name { get; } = "Discord Trigger Node";
        public DiscordTriggerNode()
        {
            IsTrigger = true;
        }
        public override async ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNull(inputs[NodeConstants.AuthorKey].GetValue<DiscordUser>());
            ArgumentNullException.ThrowIfNull(inputs[NodeConstants.ChannelKey].GetValue<DiscordChannel>());
            ArgumentNullException.ThrowIfNull(inputs[NodeConstants.MessageKey].GetValue<DiscordMessage>());

            return inputs;
        }
        public override Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs)
        {
            return Result<Dictionary<string, NodeExecutionResult>>.Success([]);
        }
    }
}
