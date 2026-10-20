using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AzertaNodes.Infrastructure.Services
{
    public class LlmService(IKernelFactory kernelFactory) : ILlmService
    {
        public async Task<Result<string>> GetTextAsync(LlmRequestObject request)
        {
            try
            {
                var kernel = kernelFactory.CreateKernel(request.ApiKey, request.ModelId, request.Endpoint);

                var history = new ChatHistory()
                {
                    new ChatMessageContent(AuthorRole.System, request.SystemPrompt ?? string.Empty),
                    new ChatMessageContent(AuthorRole.User, request.UserPrompt)
                };
                var сhatService = kernel.GetRequiredService<IChatCompletionService>();

                var result = await сhatService.GetChatMessageContentAsync(history, executionSettings: request.Settings, kernel: kernel, cancellationToken: request.CancellationToken);
                if (string.IsNullOrEmpty(result.Content))
                {
                    return Result<string>.Failure(new Error() { ErrorType = ErrorType.Failure, UserErrorMessage = "The language model did not return any content." });
                }
                return Result<string>.Success(result.Content);
            }
            catch
            {
                return Result<string>.Failure(new Error() { ErrorType = ErrorType.Failure, UserErrorMessage ="An error occurred while processing the request."});
            }
        }
    }
}
