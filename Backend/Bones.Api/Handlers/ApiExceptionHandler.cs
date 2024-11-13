using System.Text.Json;
using Bones.Api.Models;
using Bones.Shared.Backend.Models;
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
        if (exception is ForbiddenAccessException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Body = await JsonSerializer.Serialize(new ErrorResponse()
            {
                Errors = new()
                {
                    { BonesResponseBase.GENERIC_SERVER_ERROR_KEY, [BonesResponseBase.FORBIDDEN_ERROR_VALUE] }
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
                { BonesResponseBase.GENERIC_SERVER_ERROR_KEY, [exception.Message] }
            }
        }).ToStreamAsync(cancellationToken);

        return true;
    }
}