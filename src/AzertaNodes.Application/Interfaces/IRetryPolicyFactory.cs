using Polly;

namespace AzertaNodes.Application.Interfaces
{
    public interface IRetryPolicyFactory
    {
        IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(int maxAttempts, int maxJitterMilliseconds, int maxDelaySeconds);
    }
}