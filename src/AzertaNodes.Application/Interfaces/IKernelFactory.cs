using Microsoft.SemanticKernel;

namespace AzertaNodes.Application.Interfaces
{
    public interface IKernelFactory
    {
        Kernel CreateKernel(string apiKey, string modelId, string? endpointUrl = null);
    }
}
