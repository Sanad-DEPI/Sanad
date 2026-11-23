using SANAD.Application.Common;

namespace SANAD.API.Middleware;

/// <summary>
/// Maps exceptions to the shared API response envelope. Internal details are not returned.
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(exception, "Unauthorized request failed.");
            await WriteAsync(context, StatusCodes.Status401Unauthorized, "Unauthorized.");
        }
        catch (ArgumentException exception)
        {
            logger.LogWarning(exception, "Request was rejected as invalid.");
            await WriteAsync(context, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing a request.");
            await WriteAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task WriteAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(message));
    }
}
