using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using AzertaNodes.Core.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzertaNodes.Application.Implementations
{
    public class GraphService(IProjectService projectService, IGraphRunner graphRunner, IGraphValidationService graphValidationService, IUnitOfWork unitOfWork) : IGraphService
    {
        private static readonly JsonSerializerOptions DeserializeOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        public async Task<Result<Dictionary<string, object>>> ProcessAndRunAsync(Guid projectId, CancellationToken ct)
        {
            var projectResult = await projectService.GetProjectByIdAsync(projectId, ct);
            if (projectResult.IsFailure)
            {
                return Result<Dictionary<string, object>>.Failure(new Error { ErrorType = projectResult.Error!.ErrorType, UserErrorMessage = projectResult.Error.UserErrorMessage });
            }

            var project = projectResult.Value!;
            var currentGraphHash = project.ComputeCurrentHash();
            var needsValidationCheck = currentGraphHash != project.GraphHash;
            if (!string.IsNullOrEmpty(project.ErrorMessage) && !needsValidationCheck)
            {
                return Result<Dictionary<string, object>>.Failure(new Error { ErrorType = ErrorType.Failure, UserErrorMessage = project.ErrorMessage });
            }
            var graphDataResult = DeserializeGraphData(project);
            if (graphDataResult.IsFailure)
            {
                return Result<Dictionary<string, object>>.Failure(graphDataResult.Error!);
            }

            if (!project.IsSuccessfullyValidated || needsValidationCheck)
            {
                var validationResult = graphValidationService.ValidateGraph(project, graphDataResult.Value!.Nodes, graphDataResult.Value.Edges);
                if (validationResult.IsFailure)
                {
                    project.ErrorMessage = validationResult.Error!.UserErrorMessage;
                    project.ChangeValidationStatus(false);
                    await projectService.EditProjectAsync(project, ct);
                    return Result<Dictionary<string, object>>.Failure(validationResult.Error!);
                }
                project.ErrorMessage = null;
                project.ChangeValidationStatus(true);
                await projectService.EditProjectAsync(project, ct);
                await unitOfWork.ExecuteAsync(ct);
            }
            var runResult = await graphRunner.RunAsync(project, graphDataResult.Value!.Nodes.ToList(), graphDataResult.Value.Edges, ct);
            return Result<Dictionary<string, object>>.Success(runResult);
        }

        private static Result<GraphDataDto> DeserializeGraphData(Project project)
        {
            try
            {
                var result = JsonSerializer.Deserialize<GraphDataDto>(project.GraphDefinition, DeserializeOptions);
                return result is null
                    ? Result<GraphDataDto>.Failure(new Error { ErrorType = ErrorType.Failure, UserErrorMessage = UserErrorMessages.DeserializationReturnedNull })
                    : Result<GraphDataDto>.Success(result);
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
                return Result<GraphDataDto>.Failure(new Error { ErrorType = ErrorType.Failure, UserErrorMessage = UserErrorMessages.ProjectDataCorrupted });
            }
        }
    }
}