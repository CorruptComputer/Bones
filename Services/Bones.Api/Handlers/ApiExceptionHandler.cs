using System.Text.Json;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
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
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Body = await JsonSerializer.Serialize(new ErrorResponse()
            {
                Errors = new()
                {
                    { BonesResponseBase.REQUEST_ERROR_KEY, [BonesResponseBase.UNAUTHENTICATED_ERROR_VALUE] }
                }
            }).ToStreamAsync(cancellationToken);

            return true;
        }

        if (exception is ForbiddenException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Body = await JsonSerializer.Serialize(new ErrorResponse()
            {
                Errors = new()
                {
                    { BonesResponseBase.REQUEST_ERROR_KEY, [BonesResponseBase.FORBIDDEN_ERROR_VALUE] }
                }
            }).ToStreamAsync(cancellationToken);

            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.Body = await JsonSerializer.Serialize(new ErrorResponse()
        {
            Errors = new()
            {
                { BonesResponseBase.SERVER_ERROR_KEY, [exception.Message] }
            }
        }).ToStreamAsync(cancellationToken);

        return true;
    }
}