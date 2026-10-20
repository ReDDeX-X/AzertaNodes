using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;
using System.Net.Http.Headers;
using System.Text;

namespace AzertaNodes.Infrastructure.Nodes;

public class HttpRequestNode(ICredentialService credentialService, IHttpClientFactory httpClientFactory, IRetryPolicyFactory retryPolicyFactory) : BaseNode
{
    private readonly HttpClient httpClient = httpClientFactory.CreateClient("HttpRequestNode");

    public override string Name => "HTTP request node";

    private static readonly HashSet<string> ExcludedHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Host", "Content-Length", "Connection"
    };

    private static readonly HashSet<string> AllowedHttpMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS"
    };

    public override async ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var methodString = GetInput<string>(inputs, NodeConstants.RequestMethodKey);
        if (string.IsNullOrWhiteSpace(methodString) || !AllowedHttpMethods.Contains(methodString))
        {
            throw new ArgumentException($"Invalid or unsupported HTTP method: {methodString}", nameof(methodString));
        }
        var httpMethod = new HttpMethod(methodString);

        var requestUriString = GetInput<string>(inputs, NodeConstants.UriKey);
        if (string.IsNullOrWhiteSpace(requestUriString) ||
            !Uri.TryCreate(requestUriString, UriKind.Absolute, out var requestUri) ||
            (requestUri.Scheme != Uri.UriSchemeHttp && requestUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException($"Invalid request URI: {requestUriString}", nameof(requestUriString));
        }

        var isRetryEnabled = GetInput<bool?>(inputs, NodeConstants.EnablePollingKey) ?? false;
        var maxAttempts = isRetryEnabled ? (GetInput<int?>(inputs, NodeConstants.MaxPollingAttemptsKey) ?? 5) : 1;
        var maxJitterMilliseconds = isRetryEnabled ? (GetInput<int?>(inputs, NodeConstants.MaxJitterMillisecondsKey) ?? 100) : 0;
        var maxDelaySeconds = isRetryEnabled ? (GetInput<int?>(inputs, NodeConstants.MaxDelaySecondsKey) ?? 30) : 0;

        var bearerToken = await GetBearerTokenAsync(inputs, cancellationToken);
        HttpRequestMessage requestFactory() => CreateRequest(inputs, httpMethod, requestUri, bearerToken);

        using var response = isRetryEnabled
            ? await ExecuteWithRetryAsync(requestFactory, maxAttempts, maxJitterMilliseconds, maxDelaySeconds, cancellationToken)
            : await ExecuteSingleAsync(requestFactory, cancellationToken);

        var result = new Dictionary<string, NodeExecutionResult>
        {
            [NodeConstants.StatusCodeKey] = NodeExecutionResult.Create((int)response.StatusCode),
            [NodeConstants.IsSuccessStatusCodeKey] = NodeExecutionResult.Create(response.IsSuccessStatusCode)
        };
        if (response.Content != null)
        {
            result[NodeConstants.ResponseMessageKey] = NodeExecutionResult.Create(await response.Content.ReadAsStringAsync(cancellationToken));
        }

        return result;
    }

    public override Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs)
    {
        var userErrors = new List<string>();

        var methodString = GetInput<string>(inputs, NodeConstants.RequestMethodKey);
        if (methodString is null || methodString.GetType() != typeof(string) || string.IsNullOrWhiteSpace(methodString as string) || !AllowedHttpMethods.Contains(methodString))
        {
            userErrors.Add($"Invalid or unsupported HTTP method: {methodString}. Allowed methods are: {string.Join(", ", AllowedHttpMethods)}.");
        }

        var requestUriString = GetInput<string>(inputs, NodeConstants.UriKey);
        if (requestUriString is null || requestUriString.GetType() != typeof(string) || string.IsNullOrWhiteSpace(requestUriString as string) || !Uri.TryCreate(requestUriString as string, UriKind.Absolute, out _))
        {
            userErrors.Add($"Invalid request URI: {requestUriString}. Please provide a valid absolute URI.");
        }

        if (userErrors.Count > 0)
        {
            var errorMessage = string.Join("\n", userErrors);
            return Result<Dictionary<string, NodeExecutionResult>>.Failure(new Error { ErrorType = ErrorType.BadRequest, UserErrorMessage = errorMessage });
        }

        var result = new Dictionary<string, NodeExecutionResult>
        {
            [NodeConstants.StatusCodeKey] = NodeExecutionResult.Create(200),
            [NodeConstants.ResponseMessageKey] = NodeExecutionResult.Create("Test successful. The HTTP request node is configured correctly.")
        };

        return Result<Dictionary<string, NodeExecutionResult>>.Success(result);
    }

    private async Task<HttpResponseMessage> ExecuteSingleAsync(Func<HttpRequestMessage> requestFactory, CancellationToken cancellationToken)
    {
        using var request = requestFactory();
        return await httpClient.SendAsync(request, cancellationToken);
    }

    private async Task<HttpResponseMessage> ExecuteWithRetryAsync(Func<HttpRequestMessage> requestFactory, int maxAttempts, int maxJitterMilliseconds, int maxDelaySeconds, CancellationToken cancellationToken)
    {
        var retryPolicy = retryPolicyFactory.CreateRetryPolicy(maxAttempts, maxJitterMilliseconds: maxJitterMilliseconds, maxDelaySeconds: maxDelaySeconds);

        return await retryPolicy.ExecuteAsync(async ct =>
        {
            using var request = requestFactory();
            var response = await httpClient.SendAsync(request, ct);

            return response;
        }, cancellationToken);
    }

    private static HttpRequestMessage CreateRequest(Dictionary<string, NodeExecutionResult> inputs, HttpMethod method, Uri uri, string? bearerToken)
    {
        var request = new HttpRequestMessage(method, uri);

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var body = GetInput<string>(inputs, NodeConstants.BodyKey);
        if (!string.IsNullOrEmpty(body))
        {
            var contentType = GetInput<string>(inputs, NodeConstants.ContentTypeKey) ?? "text/plain";
            request.Content = new StringContent(body, Encoding.UTF8, contentType);
        }

        AddHeaders(inputs, request);

        return request;
    }

    private async Task<string?> GetBearerTokenAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
    {
        var authorization = GetInput<string>(inputs, NodeConstants.AuthorizationKey);
        if (!string.IsNullOrWhiteSpace(authorization) && Guid.TryParse(authorization, out var credentialGuid))
        {
            var credentialResult = await credentialService.GetDecryptedCredentialAsync(credentialGuid, cancellationToken);
            if (credentialResult?.IsSuccess == true && !string.IsNullOrWhiteSpace(credentialResult.Value))
            {
                return credentialResult.Value;
            }
        }
        return null;
    }

    private static void AddHeaders(Dictionary<string, NodeExecutionResult> inputs, HttpRequestMessage request)
    {
        if (!inputs.TryGetValue(NodeConstants.HeadersKey, out var headersObj) || headersObj.Value is not Dictionary<string, string> headers)
        {
            return;
        }

        foreach (var (key, value) in headers)
        {
            if (ExcludedHeaders.Contains(key))
                continue;

            if (request.Headers.TryAddWithoutValidation(key, value))
                continue;

            if (request.Content != null)
            {
                if (!request.Content.Headers.TryAddWithoutValidation(key, value))
                {
                    throw new InvalidOperationException($"Invalid header: {key}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Header '{key}' requires request content, but no content was provided.");
            }
        }
    }
}
