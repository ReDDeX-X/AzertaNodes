using AzertaNodes.Api.Extensions;
using AzertaNodes.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzertaNodes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/{userId:guid}/{projectId:guid}")]
    public class RunController(IGraphService graphService) : ControllerBase
    {
        [AcceptVerbs("GET", "POST", "PUT", "DELETE", "PATCH")]
        public async Task<IActionResult> ProccessAndRunAsync(Guid projectId, CancellationToken cancellationToken)
        {
            var runResult = await graphService.ProcessAndRunAsync(projectId, cancellationToken);
            if (runResult.IsFailure)
            {
                return runResult.ToErrorResult();
            }

            var value = runResult.Value;
            if (value is null || value.Count == 0)
            {
                return NoContent();
            }
            return Ok(value);
        }

        [Authorize]
        [Route("manual")]
        [AcceptVerbs("GET", "POST", "PUT", "DELETE", "PATCH")]
        public async Task<IActionResult> ProccessAndRunManuallyAsync(Guid projectId, CancellationToken cancellationToken)
        {
            var runResult = await graphService.ProcessAndRunAsync(projectId, cancellationToken);
            if (runResult.IsFailure)
            {
                return runResult.ToErrorResult();
            }
            var value = runResult.Value;
            if (value is null || value.Count == 0)
            {
                return NoContent();
            }
            return Ok(value);
        }
    }
}
