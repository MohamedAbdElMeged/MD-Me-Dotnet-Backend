using Microsoft.AspNetCore.Diagnostics;

namespace Backend.Exceptions;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        
        // httpContext.Response.StatusCode = StatusCodes.Status303SeeOther;
        return false;
    }
}