using System;
using System.Collections.Generic;
using System.Text;

namespace AzertaNodes.Application.Implementations
{
    internal interface IHttpService
    {
        Task<HttpResponseMessage> SendAsync(CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> SendWithRetryAsync(int maxAttempts, CancellationToken cancellationToken = default);
    }
}
