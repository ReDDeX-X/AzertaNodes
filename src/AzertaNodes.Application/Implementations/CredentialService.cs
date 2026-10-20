using AutoMapper;
using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using AzertaNodes.Core.Interfaces;

namespace AzertaNodes.Application.Implementations
{
    public class CredentialService(ICredentialRepository credentialRepository, ICredentialEncryptor credentialEncryptor, IMapper mapper) : ICredentialService
    {
        public async Task<Result<string>> GetDecryptedCredentialAsync(Guid credentialId, CancellationToken cancellationToken)
        {
            //do refactoring to get userId from context
            var result = await credentialRepository.GetCredentialByIdAsync(credentialId, Guid.NewGuid(), cancellationToken);
            if (result.IsFailure)
            {
                return Result<string>.Failure(result.Error!);
            }
            if (result.Value is null)
            {
                return Result<string>.Failure(new Error
                {
                    ErrorType = ErrorType.NotFound,
                    UserErrorMessage = UserErrorMessages.CredentialsNotFound
                });
            }
            try
            {
                var decryptedKey = credentialEncryptor.Decrypt(
                    result.Value.EncryptedValue,
                    result.Value.InitializationVector);
                if (string.IsNullOrWhiteSpace(decryptedKey))
                {
                    return Result<string>.Failure(new Error
                    {
                        ErrorType = ErrorType.Validation,
                        UserErrorMessage = UserErrorMessages.DecryptedKeyEmptyOrIncorrect
                    });
                }
                return Result<string>.Success(decryptedKey);
            }
            catch
            {
                return Result<string>.Failure(new Error
                {
                    ErrorType = ErrorType.Failure,
                    UserErrorMessage = UserErrorMessages.FailedToDecryptData
                });
            }
        }

        public async Task<Result<IEnumerable<CredentialDto>>> GetCredentialsByUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var credentialsResult = await credentialRepository.GetCredentialsByUserIdAsync(userId, cancellationToken);
            if (credentialsResult.IsFailure)
            {
                return Result<IEnumerable<CredentialDto>>.Failure(credentialsResult.Error!);
            }
            var credentialDtos = mapper.Map<IEnumerable<CredentialDto>>(credentialsResult.GetValueOrThrow());
            return Result<IEnumerable<CredentialDto>>.Success(credentialDtos);
        }

        public async Task<Result<Credential>> SaveCredentialAsync(string name, string provider, string apiKey, CancellationToken cancellationToken)
        {
            (var encryptedValue, var IV) = credentialEncryptor.Encrypt(apiKey);
            var credential = new Credential
            {
                Name = name,
                Provider = provider,
                EncryptedValue = encryptedValue,
                InitializationVector = IV,
                UserId = Guid.NewGuid()
            };
            var result = await credentialRepository.SaveCredentialAsync(credential, cancellationToken);
            if (result.IsFailure)
            {
                return Result<Credential>.Failure(result.Error!);
            }
            return Result<Credential>.Success(result.Value!);
        }
        public async Task<Result<bool>> DeleteCredentialAsync(Guid credentialId, CancellationToken cancellationToken)
        { 
            var result = await credentialRepository.DeleteCredentialAsync(credentialId, Guid.NewGuid(), cancellationToken);

            if (result.IsFailure) 
            {
                return Result<bool>.Failure(result.Error!);
            }

            return Result<bool>.Success(result.Value!);
        }
    }
}
