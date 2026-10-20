using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AzertaNodes.Application.DTOs
{
    public class LlmRequestObject
    {
        public string? SystemPrompt { get; set; }
        public required string UserPrompt { get; set; }
        public required string ApiKey { get; set; }
        public required string ModelId { get; set; }
        public required CancellationToken CancellationToken { get; set; }
        public string? Endpoint { get; set; }
        public OpenAIPromptExecutionSettings? Settings { get; set; }
    }
}
