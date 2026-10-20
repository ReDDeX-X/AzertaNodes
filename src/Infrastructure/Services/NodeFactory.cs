using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using AzertaNodes.Core.Entities.Node;
using AzertaNodes.Core.Interfaces;
using AzertaNodes.Infrastructure.Nodes;
using Microsoft.Extensions.DependencyInjection;

namespace AzertaNodes.Infrastructure.Services
{
    public class NodeFactory(IServiceProvider serviceProvider) : INodeFactory
    {
        public INode Create(NodeType nodeType, Project project)
        {
            INode node = nodeType switch
            {
                NodeType.LlmNode => serviceProvider.GetRequiredService<LlmNode>(),
                NodeType.ConsoleWriteLine => serviceProvider.GetRequiredService<TestConsoleWriteLineNode>(),
                NodeType.HttpRequestNode => serviceProvider.GetRequiredService<HttpRequestNode>(),
                NodeType.HttpResponseNode => serviceProvider.GetRequiredService<HttpResponseNode>(),
                NodeType.LogicNode => serviceProvider.GetRequiredService<LogicNode>(),
                _ => throw new NotSupportedException($"Node type {nodeType} is not supported")
            };

            if (node is BaseNode baseNode)
            {
               var metadata = new NodeMetaData
               {
                   ProjectId = project.Id,
                   CredentialId = project.MainCredentialId
               };
                baseNode.MetaData = metadata;
            }

            return node;
        }
    }
}
