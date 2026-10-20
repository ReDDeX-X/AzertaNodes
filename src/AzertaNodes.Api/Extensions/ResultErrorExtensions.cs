using AzertaNodes.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AzertaNodes.Api.Extensions
{
    public static class ResultErrorExtensions
    {
        public static IActionResult ToErrorResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Cannot convert a successful result to an error result.");
            }

            var error = result.Error!;

            return error.ErrorType switch
            {
                ErrorType.Validation => new BadRequestObjectResult(CreateProblemDetails("Validation Error", StatusCodes.Status400BadRequest, error)),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(CreateProblemDetails("Unauthorized", StatusCodes.Status401Unauthorized, error)),
                ErrorType.Forbidden => new ObjectResult(CreateProblemDetails("Forbidden", StatusCodes.Status403Forbidden, error)) { StatusCode = 403 },
                ErrorType.NotFound => new NotFoundObjectResult(CreateProblemDetails("Not Found", StatusCodes.Status404NotFound, error)),
                ErrorType.MethodNotAllowed => new ObjectResult(CreateProblemDetails("Method Not Allowed", StatusCodes.Status405MethodNotAllowed, error)) { StatusCode = 405 },
                ErrorType.Conflict => new ConflictObjectResult(CreateProblemDetails("Conflict", StatusCodes.Status409Conflict, error)),
                ErrorType.Failure => new ObjectResult(CreateProblemDetails("Failure", StatusCodes.Status500InternalServerError, error)) { StatusCode = 500 },
                ErrorType.BadRequest => new BadRequestObjectResult(CreateProblemDetails("Bad Request", StatusCodes.Status400BadRequest, error)),
                _ => new ObjectResult(CreateProblemDetails("Server Error", StatusCodes.Status500InternalServerError, error)) { StatusCode = 500 }
            };
        }

        private static ProblemDetails CreateProblemDetails(string title, int status, Error error)
        {
            var problemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = error.UserErrorMessage,
            };
            return problemDetails;
        }
    }
}