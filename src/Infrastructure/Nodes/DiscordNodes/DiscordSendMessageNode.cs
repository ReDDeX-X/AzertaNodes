using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;

namespace AzertaNodes.Infrastructure.Nodes.DiscordNodes
{
    public class DiscordSendMessageNode(IDiscordConnectionManager discordConnectionManager) : BaseNode
    {
        public override string Name { get; } = "Discord Send Message Node";
        public override async ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var message = GetInput<string>(inputs, NodeConstants.MessageKey);
            ArgumentNullException.ThrowIfNullOrEmpty(message, nameof(message));
            var channelId = GetInput<ulong>(inputs, NodeConstants.TargetChannelKey);
            if (channelId == default) throw new ArgumentNullException(nameof(channelId));
            if (!MetaData.CredentialId.HasValue) throw new ArgumentNullException(nameof(MetaData.CredentialId));

            var client = await discordConnectionManager.GetOrCreateClientAsync(MetaData.ProjectId, MetaData.CredentialId.Value, cancellationToken);
            var channel = await client.GetChannelAsync(channelId);
            var responseMessage = await client.SendMessageAsync(channel, message);
            return new Dictionary<string, NodeExecutionResult>
            {
                [NodeConstants.ResponseMessageKey] = NodeExecutionResult.Create(responseMessage)
            };
        }
        public override Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs)
        {
            return Result<Dictionary<string, NodeExecutionResult>>.Success([]);
        }
    }
}