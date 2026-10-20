using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using AzertaNodes.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AzertaNodes.Infrastructure.Persistence
{
    public class CredentialRepository(AppDbContext dbContext) : ICredentialRepository
    {
        public async Task<Result<Credential>> GetCredentialByIdAsync(Guid credentialId, Guid userId, CancellationToken cancellationToken)
        {
            var credential = await dbContext.CredentialSet
                .FindAsync([credentialId], cancellationToken: cancellationToken);

            if (credential is null || credential.UserId != userId)
            {
                return Result<Credential>.Failure(new Error() { ErrorType = ErrorType.NotFound, UserErrorMessage = "Credential not found." });
            }
            return Result<Credential>.Success(credential);
        }

        public async Task<Result<IEnumerable<Credential>>> GetCredentialsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var credentials = await dbContext.CredentialSet
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<Credential>>.Success(credentials);
        }
        public async Task<Result<Credential>> SaveCredentialAsync(Credential credential, CancellationToken cancellationToken)
        {
            bool exists = await dbContext.CredentialSet.AnyAsync(c => c.Id == credential.Id, cancellationToken);
            if (exists)
            {
                return Result<Credential>.Failure(new Error { ErrorType = ErrorType.Conflict, UserErrorMessage = "Credential with the same ID already exists." });
            }

            await dbContext.CredentialSet.AddAsync(credential, cancellationToken);
            return Result<Credential>.Success(credential);
        }

        public async Task<Result<bool>> DeleteCredentialAsync(Guid credentialId, Guid userId, CancellationToken cancellationToken)
        {
            var credential = await dbContext.CredentialSet.FindAsync([credentialId], cancellationToken: cancellationToken);
            if (credential is null || credential.UserId != userId)
            {
                return Result<bool>.Failure(new Error() { ErrorType = ErrorType.NotFound, UserErrorMessage = "Credential not found." });
            }
            dbContext.CredentialSet.Remove(credential);
            return Result<bool>.Success(true);
        }
    }
}
