using Backend.Results;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.NoContent();

        return controller.Problem(
            statusCode: MapStatusCode(result.Error!),
            title: result.Error!.Code,
            detail: result.Error.Message
        );
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return controller.Problem(
            statusCode: MapStatusCode(result.Error!),
            title: result.Error!.Code,
            detail: result.Error.Message
        );
    }

    private static int MapStatusCode(Error error) =>
        error.ErrorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
}