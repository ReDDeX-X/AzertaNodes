using AzertaNodes.Application.Interfaces;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;

namespace AzertaNodes.Infrastructure.Services
{
    public class KernelFactory(IHttpClientFactory httpClientFactory) : IKernelFactory
    {
        public Kernel CreateKernel(string apiKey, string modelId, string? endpointUrl)
        {
            var httpClient = httpClientFactory.CreateClient("Default");

            var openAIClientOptions = new OpenAIClientOptions
            {
                Transport = new System.ClientModel.Primitives.HttpClientPipelineTransport(httpClient),
            };
            if (endpointUrl is not null)
            {
                openAIClientOptions.Endpoint = new Uri(endpointUrl);
            }
            var client = new OpenAIClient(new ApiKeyCredential(apiKey), openAIClientOptions);
            var builder = Kernel.CreateBuilder();

            builder.AddOpenAIChatCompletion(modelId, client);
            return builder.Build();
        }
    }
}
