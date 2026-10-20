
using System.Text.Json.Serialization;

namespace AzertaNodes.Application.DTOs
{
    public class ProjectDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? CredentialId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? UserId { get; set; }
        public required bool IsAsync { get; set; }
        public required bool IsSuccessfullyValidated { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorMessage { get; set; }
    }
}
