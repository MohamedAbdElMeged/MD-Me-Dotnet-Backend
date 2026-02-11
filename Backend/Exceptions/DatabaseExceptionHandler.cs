using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.Exceptions;

public class DatabaseExceptionHandler(IProblemDetailsService problemDetailsService,
    ILogger<DatabaseExceptionHandler> _logger) : IExceptionHandler
{
    public async  ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateException dbUpdateException)
        {
            return false;
        }

        _logger.LogError(exception, $"Database Exception Handler, traceId: {httpContext.TraceIdentifier}");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Detail = "One or more database errors occurred",
                Status = StatusCodes.Status500InternalServerError
            }
        };
        return await problemDetailsService.TryWriteAsync(context);
    }
}