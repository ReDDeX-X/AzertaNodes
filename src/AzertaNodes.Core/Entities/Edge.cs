namespace AzertaNodes.Core.Entities
{
    public class Edge
    {
        public Guid Id { get; set; }
        public required Guid SourceNodeId { get; set; }
        public required string OutputKey { get; set; }

        public required Guid TargetNodeId { get; set; }
        public required string InputKey { get; set; }
        public required string DataType { get; set; }
    }
}
