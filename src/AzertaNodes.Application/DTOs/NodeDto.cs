using AzertaNodes.Core.Entities;

namespace AzertaNodes.Application.DTOs
{
    public class NodeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public NodeType Type { get; set; }
        public bool IsOutput { get; set; } = false;
        public bool IsTrigger { get; set; } = false;
        public bool StopGraphOnFailure { get; set; } = true;
        public bool IsAsync { get; set; } = false;
        public bool IsSync { get => !IsAsync; }
        public bool IsWaitAll { get; set; } = true;
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public Dictionary<string, object> Settings { get; set; } = [];
    }
}
