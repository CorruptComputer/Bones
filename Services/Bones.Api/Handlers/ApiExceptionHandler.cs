using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Bones.Api.Handlers;

/// <summary>
///   Handles exceptions thrown by the API before they are returned to the user
/// </summary>
public class ApiExceptionHandler : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is UnauthenticatedException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponse()
            {
                Errors = new()
                {
                    { BonesResponseBase.REQUEST_ERROR_KEY, [BonesResponseBase.UNAUTHENTICATED_ERROR_VALUE] }
                }
            }, cancellationToken);

            return true;
        }

        if (exception is ForbiddenException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponse()
            {
                Errors = new()
                {
                    { BonesResponseBase.REQUEST_ERROR_KEY, [BonesResponseBase.FORBIDDEN_ERROR_VALUE] }
                }
            }, cancellationToken);

            return true;
        }

        if (exception is BadRequestException badRequestException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponse($"Bad Request '{badRequestException.RequestModel}'")
            {
                Errors = new()
                {
                    { badRequestException.BadField, [badRequestException.Message] }
                },
            }, cancellationToken);

            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse()
        {
            Errors = new()
            {
                { BonesResponseBase.SERVER_ERROR_KEY, [exception.Message] }
            }
        }, cancellationToken);

        return true;
    }
}