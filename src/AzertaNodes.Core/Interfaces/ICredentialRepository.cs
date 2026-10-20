using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Core.Interfaces
{
    public interface ICredentialRepository
    {
        public Task<Result<Credential>> GetCredentialByIdAsync(Guid credentialId, Guid userId, CancellationToken cancellationToken);
        public Task<Result<IEnumerable<Credential>>> GetCredentialsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        public Task<Result<Credential>> SaveCredentialAsync(Credential credential, CancellationToken cancellationToken);
        public Task<Result<bool>> DeleteCredentialAsync(Guid credentialId, Guid userId, CancellationToken cancellationToken);
    }
}
