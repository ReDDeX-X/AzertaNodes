using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using AzertaNodes.Core.Interfaces;

namespace AzertaNodes.Application.Interfaces
{
    public interface INodeFactory
    {
        public INode Create(NodeType nodeType, Project project);
    }
}
