using AzertaNodes.Application.DTOs;

namespace AzertaNodes.Application.Interfaces
{
    public interface IVideoProvider
    {
        public string ProviderName { get; }
        public Task<VideoGenerationResult> GenerateAsync(VideoGenerationRequest request, string apiKey);
    }
}
