using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AzertaNodes.Infrastructure.Nodes
{
    public class LlmNode(ILlmService llmService, ICredentialService credentialService) : BaseNode
    {
        public override string Name { get; } = "LLM node";
        public override async ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, NodeExecutionResult>();
            var credentialId = GetInput<string>(inputs, NodeConstants.CredentialIdKey);
            if (!Guid.TryParse(credentialId, out var credentialGuid))
            {
                throw new ArgumentException("Invalid credential ID format.", nameof(credentialId));
            }
            var apiKeyResult = await credentialService.GetDecryptedCredentialAsync(credentialGuid, cancellationToken);
            if (apiKeyResult.IsFailure || apiKeyResult.Value is null)
            {
                throw new InvalidOperationException("Failed to retrieve API key.");
            }

            var apiKey = apiKeyResult.Value;

            var userPrompt = GetInput<string>(inputs, NodeConstants.UserPromptKey);
            if (string.IsNullOrEmpty(userPrompt))
            {
                throw new ArgumentException(nameof(userPrompt));
            }
            var modelId = GetInput<string>(inputs, NodeConstants.ModelIdKey);
            if (string.IsNullOrEmpty(modelId))
            {
                throw new ArgumentException(nameof(modelId));
            }
            var systemPrompt = GetInput<string>(inputs, NodeConstants.SystemPromptKey); 
            var endpoint = GetInput<string>(inputs, NodeConstants.EndpointKey);
            var settings = GetOpenAiSettings(inputs);

            var request = new LlmRequestObject
            {
                CancellationToken = cancellationToken,
                SystemPrompt = systemPrompt,
                UserPrompt = userPrompt,
                ModelId = modelId,
                ApiKey = apiKey,
                Endpoint = endpoint,
                Settings = settings
            };

            result[NodeConstants.LlmNodeResultKey] = NodeExecutionResult.Create(await llmService.GetTextAsync(request));
            return result;
        }

        private static OpenAIPromptExecutionSettings GetOpenAiSettings(Dictionary<string, NodeExecutionResult> inputs)
        {
            var settings = new OpenAIPromptExecutionSettings();

            if (GetInput<double?>(inputs, NodeConstants.TemperatureKey) is double tempValue)
                settings.Temperature = tempValue;
            if (GetInput<int?>(inputs, NodeConstants.MaxTokensKey) is int maxTokensValue)
                settings.MaxTokens = maxTokensValue;
            if (GetInput<double?>(inputs, NodeConstants.TopPKey) is double tpValue)
                settings.TopP = tpValue;
            if (GetInput<double?>(inputs, NodeConstants.PresencePenaltyKey) is double ppValue)
                settings.PresencePenalty = ppValue;
            if (GetInput<double?>(inputs, NodeConstants.FrequencyPenaltyKey) is double fpValue)
                settings.FrequencyPenalty = fpValue;
            if (GetInput<int?>(inputs, NodeConstants.SeedKey) is int seedValue)
                settings.Seed = seedValue;
            if (GetInput<string>(inputs, NodeConstants.ServiceIdKey) is string serviceIdValue)
                settings.ServiceId = serviceIdValue;
            if (GetInput<IList<string>>(inputs, NodeConstants.StopSequencesKey) is IList<string> ssValue)
                settings.StopSequences = ssValue;

            return settings;
        }
        public override Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs)
        {
            var userErrors = new List<string>();
            var credentialId = inputs.TryGetValue(NodeConstants.CredentialIdKey, out var credId) ? credId : null;
            if (credentialId == null || credentialId.GetType() != typeof(Guid))
            {
                if (credentialId is null)
                {
                    userErrors.Add($"'{NodeConstants.CredentialIdKey}' is required.");
                }
                else
                {
                    userErrors.Add($"'{NodeConstants.CredentialIdKey}' must be Guid.");
                }
            }

            var userPrompt = GetInput<string>(inputs, NodeConstants.UserPromptKey);
            if (userPrompt == null)
            {
                userErrors.Add($"'{NodeConstants.UserPromptKey}' is required.");
            }

            var modelId = GetInput<string>(inputs, NodeConstants.ModelIdKey);
            if (modelId == null)
            {
                 userErrors.Add($"'{NodeConstants.ModelIdKey}' is required.");
            }

            if (userErrors.Count > 0)
            {
                var errorMessage = string.Join("\n", userErrors);
                return Result<Dictionary<string, NodeExecutionResult>>.Failure(new Error { ErrorType = ErrorType.BadRequest, UserErrorMessage = errorMessage });
            }

            var result = new Dictionary<string, NodeExecutionResult>
            {
                [NodeConstants.LlmNodeResultKey] = NodeExecutionResult.Create("Test response from LLM node.")
            };
            return Result<Dictionary<string, NodeExecutionResult>>.Success(result);
        }
    }
}
