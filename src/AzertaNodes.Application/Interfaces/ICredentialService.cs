using AzertaNodes.Application.DTOs;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Application.Interfaces
{
    public interface ICredentialService
    {
        Task<Result<string>> GetDecryptedCredentialAsync(Guid credentialId, CancellationToken cancellationToken);
        Task<Result<IEnumerable<CredentialDto>>> GetCredentialsByUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<Result<Credential>> SaveCredentialAsync(string name, string provider, string apiKey, CancellationToken cancellationToken);
        Task<Result<bool>> DeleteCredentialAsync(Guid credentialId, CancellationToken cancellationToken);
    }
}
