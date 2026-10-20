using AzertaNodes.Core.Entities;

namespace AzertaNodes.Application.DTOs
{
    public class GraphDataDto
    {
        public required List<NodeDto> Nodes { get; set; }
        public required List<Edge> Edges { get; set; }
    }
}
