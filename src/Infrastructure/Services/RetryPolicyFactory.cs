using AzertaNodes.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net;

namespace AzertaNodes.Infrastructure.Services
{

    public class RetryPolicyFactory(ILogger<RetryPolicyFactory> logger) : IRetryPolicyFactory
    {

        public IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(int maxAttempts, int maxJitterMilliseconds, int maxDelaySeconds)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested)
                .OrResult(r =>
                {
                    if ((int)r.StatusCode >= 500 || r.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        r.Dispose();
                        return true;
                    }
                    return false;
                })
                .WaitAndRetryAsync(
                    maxAttempts,
                    retryAttempt =>
                    {
                        var baseDelay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1));
                        var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, maxJitterMilliseconds));

                        var delay = baseDelay + jitter;

                        return delay > TimeSpan.FromSeconds(maxDelaySeconds)
                            ? TimeSpan.FromSeconds(maxDelaySeconds)
                            : delay;
                    },
                    onRetry: (outcome, timespan, retryNumber, context) =>
                    {
                        outcome.Result?.Dispose();

                        logger.LogWarning(
                            outcome.Exception,
                            "Retry {RetryNumber} after {Delay}s. StatusCode: {StatusCode}",
                            retryNumber,
                            timespan.TotalSeconds,
                            outcome.Result?.StatusCode);
                    });
        }
    }
}