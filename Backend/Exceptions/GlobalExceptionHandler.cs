using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Exceptions;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = exception.Message,
                Detail = "An error occurred while processing your request.",
                Status = StatusCodes.Status500InternalServerError
            }
        };
        return await problemDetailsService.TryWriteAsync(context);
        
    }
    
}