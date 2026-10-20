using AutoMapper;
using AzertaNodes.Api.Extensions;
using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using AzertaNodes.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AzertaNodes.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]/{userId:guid}/{projectId:guid}")]
    public class ProjectController(IProjectService projectService, IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetProjectAsync(Guid projectId, CancellationToken cancellationToken)
        {
            var projectResult = await projectService.GetProjectByIdAsync(projectId, cancellationToken);
            if (projectResult.IsFailure)
                return projectResult.ToErrorResult();

            var project = projectResult.GetValueOrThrow();
            return Ok(mapper.Map<ProjectDto>(project));
        }
        [Route("/api/[controller]/{userId:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetProjects(Guid userId, CancellationToken cancellationToken)
        {
            var projectsResult = await projectService.GetProjectsByUserIdAsync(userId, cancellationToken);
            if (projectsResult.IsFailure)
                return projectsResult.ToErrorResult();
            var projects = projectsResult.GetValueOrThrow();
            var response = mapper.Map<IEnumerable<ProjectDto>>(projects);
            return Ok(response);
        }
        [Route("/api/[controller]/{userId:guid}")]
        [HttpPost]
        public async Task<IActionResult> CreateProject(Guid userId, [FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
        {
            var project = new Project()
            {
                Name = request.Name,
                UserId = userId,
                Description = request.Description
            };
            var projectResult = await projectService.CreateProjectAsync(project, cancellationToken);
            if (projectResult.IsFailure)
                return projectResult.ToErrorResult();

            await unitOfWork.ExecuteAsync(cancellationToken);
            var createdProject = projectResult.GetValueOrThrow();

            return CreatedAtAction(nameof(GetProjectAsync), new { userId, projectId = createdProject.Id }, mapper.Map<ProjectDto>(createdProject));
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] PatchProjectRequest request, CancellationToken cancellationToken)
        {
            var projectResult = await projectService.GetProjectByIdAsync(projectId, cancellationToken);
            if (projectResult.IsFailure)
                return projectResult.ToErrorResult();

            var project = projectResult.GetValueOrThrow();
            if (request.Name.HasValue)
            {
                var newValue = request.Name.Value.GetString();
                if (string.IsNullOrWhiteSpace(newValue))
                {
                    return BadRequest("Project name cannot be empty.");
                }
                project.Name = newValue;
            }

            if (request.Description.HasValue)
            {
                project.Description = request.Description.Value.ValueKind == JsonValueKind.Null
                    ? null
                    : request.Description.Value.GetString();
            }

            var graphToUpdate = request.GraphDefinition.HasValue
                ? request.GraphDefinition
                : null;

            var updateResult = await projectService.EditProjectAsync(project, cancellationToken, graphToUpdate);

            if (updateResult.IsFailure)
                return updateResult.ToErrorResult();

            await unitOfWork.ExecuteAsync(cancellationToken);

            var updatedProject = updateResult.GetValueOrThrow();
            return Ok(mapper.Map<ProjectDto>(updatedProject));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellationToken)
        {
            var deleteResult = await projectService.DeleteProjectAsync(projectId, cancellationToken);
            if (deleteResult.IsFailure)
                return deleteResult.ToErrorResult();
            await unitOfWork.ExecuteAsync(cancellationToken);

            return NoContent();
        }

        public record CreateProjectRequest([Required] string Name, string? Description);
        public record PatchProjectRequest(JsonElement? Name, JsonElement? Description, JsonElement? GraphDefinition);
    }
}
