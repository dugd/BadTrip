using BadTrip.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BadTrip.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path,
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    problemDetails.Title = "Validation Failed";
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Detail = validationEx.Message ?? "One or more validation errors occurred.";

                    problemDetails.Extensions["errors"] = validationEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    break;

                case NotFoundException notFoundEx:
                    problemDetails.Title = "Resource Not Found";
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Detail = notFoundEx.Message;
                    break;

                case DomainException domainEx:
                    problemDetails.Title = "Business Rule Violation";
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    problemDetails.Detail = domainEx.Message;
                    break;

                case ForbiddenException forbiddenEx:
                    problemDetails.Title = "Forbidden";
                    problemDetails.Status = StatusCodes.Status403Forbidden;
                    problemDetails.Detail = forbiddenEx.Message;
                    break;

                // usually at auth middleware
                case UnauthorizedAccessException:
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Detail = "You are not authorized to access this resource.";
                    break;

                default:
                    problemDetails.Title = "Internal Server Error";
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Detail = "Something went wrong. We are looking into it.";
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync( problemDetails, cancellationToken );

            return true;
        }
    }
}
