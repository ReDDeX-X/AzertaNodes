using AzertaNodes.Application.DTOs;
using AzertaNodes.Core.Entities;

namespace AzertaNodes.Application.Interfaces
{
    public interface ILlmService
    {
        Task<Result<string>> GetTextAsync(LlmRequestObject request);
    }
}
