using AzertaNodes.Api.Extensions;
using AzertaNodes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AzertaNodes.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]/{userId:guid}")]
    public class CredentialController(ICredentialService credentialService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCredentialsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var credentialsResult = await credentialService.GetCredentialsByUserAsync(userId, cancellationToken);
            if (credentialsResult.IsFailure)
                return credentialsResult.ToErrorResult();

            var credentials = credentialsResult.GetValueOrThrow();
            return Ok(credentials);
        }
        [HttpPost]
        public async Task<IActionResult> SaveCredentialAsync([FromBody] SaveCredentialRequest request, CancellationToken cancellationToken)
        {
            var saveResult = await credentialService.SaveCredentialAsync(request.Name, request.Provider, request.Key, cancellationToken);
            if (saveResult.IsFailure)
                return saveResult.ToErrorResult();

            var result = saveResult.GetValueOrThrow();
            return Ok(result.Id);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCredentialAsync(Guid credentialId, CancellationToken cancellationToken)
        {
            var deleteResult = await credentialService.DeleteCredentialAsync(credentialId, cancellationToken);
            if (deleteResult.IsFailure)
                return deleteResult.ToErrorResult();

            return NoContent();
        }
        public record SaveCredentialRequest(string Name, string Provider, string Key);
    }
}
