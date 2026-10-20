using AzertaNodes.Application.Converters;
using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;
using AzertaNodes.Core.Interfaces;
using Azure.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzertaNodes.Application.Implementations
{
    public class ProjectService(IProjectRepository projectRepository, IGraphValidationService graphValidationService) : IProjectService
    {
        private const string NOT_FOUND_USER_ERROR_MESSAGE = "Project not found.";
        private const string SERVER_EXCEPTION_USER_ERROR_MESSAGE = "Something went wrong. Please try again later.";
        private readonly JsonSerializerOptions serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(),
                new ObjectPrimitiveConverter()
            }
        };
        public async Task<Result<Project>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            try
            {
                var project = await projectRepository.GetProjectByIdAsync(projectId, cancellationToken);
                if (project is null)
                {
                    return Result<Project>.Failure(new Error { ErrorType = ErrorType.NotFound, UserErrorMessage = NOT_FOUND_USER_ERROR_MESSAGE });
                }
                return Result<Project>.Success(project);
            }
            catch
            {
                return Result<Project>.Failure(new Error { ErrorType = ErrorType.Failure, UserErrorMessage = SERVER_EXCEPTION_USER_ERROR_MESSAGE });
            }
        }

        public async Task<Result<IEnumerable<Project>>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var projects = await projectRepository.GetProjectsByUserIdAsync(userId, cancellationToken);
                return Result<IEnumerable<Project>>.Success(projects);
            }
            catch
            {
                return Result<IEnumerable<Project>>.Failure(new Error { ErrorType = ErrorType.Failure, UserErrorMessage = SERVER_EXCEPTION_USER_ERROR_MESSAGE });
            }
        }
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken)
        {
            var projects = await projectRepository.GetActiveProjectsAsync(cancellationToken);
            if (projects is null)
            {
                return [];
            }
            return projects;
        }
        public async Task<Result<Project>> CreateProjectAsync(Project project, CancellationToken cancellationToken)
        {
            try
            {
                await projectRepository.SaveProjectAsync(project, cancellationToken);
                return Result<Project>.Success(project);
            }
            catch
            {
                return Result<Project>.Failure(new Error { ErrorType = ErrorType.Failure, UserErrorMessage = SERVER_EXCEPTION_USER_ERROR_MESSAGE });
            }
        }

        public async Task<Result<Project>> EditProjectAsync(Project project, CancellationToken cancellationToken, JsonElement? graphDefinition = null)
        {
            try
            {
                if (graphDefinition.HasValue)
                {
                    project.ErrorMessage = null;
                    try
                    {
                        var rawGraphDefinition = graphDefinition.Value.GetRawText();
                        var graphData = JsonSerializer.Deserialize<GraphDataDto>(rawGraphDefinition, serializerOptions);

                        if (graphData is not null)
                        {
                            var result = graphValidationService.ValidateGraph(project, graphData.Nodes, graphData.Edges);
                            if (result.IsFailure)
                            {
                                project.EditGraphDefinition(rawGraphDefinition, false);
                                project.ErrorMessage = result.Error!.UserErrorMessage;
                            }
                            else
                            {
                                project.EditGraphDefinition(rawGraphDefinition, true);
                            }
                        }
                        else
                        {
                            project.ErrorMessage = "Graph definition is blank";
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        project.ErrorMessage = "Invalid graph JSON";
                    }
                }
                await projectRepository.EditProjectAsync(project, cancellationToken);
                return Result<Project>.Success(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Result<Project>.Failure(new Error { ErrorType = ErrorType.Failure, UserErrorMessage = SERVER_EXCEPTION_USER_ERROR_MESSAGE });
            }
        }

        public async Task<Result<bool>> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken)
        {
            try
            {
                var project = await projectRepository.GetProjectByIdAsync(projectId, cancellationToken);
                if (project is null)
                {
                    return Result<bool>.Failure(new Error { ErrorType = ErrorType.NotFound, UserErrorMessage = NOT_FOUND_USER_ERROR_MESSAGE });
                }
                await projectRepository.DeleteProjectAsync(projectId, cancellationToken);
                return Result<bool>.Success(true);
            }
            catch
            {
                return Result<bool>.Failure(new Error { ErrorType = ErrorType.Failure, UserErrorMessage = SERVER_EXCEPTION_USER_ERROR_MESSAGE });
            }
        }
    }
}
