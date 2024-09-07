using Bones.Api.Models;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Handlers;

/// <summary>
/// 
/// </summary>
public class ApiExceptionHandler : IExceptionHandler
{
    /// <summary>
    ///   Handles exceptions that bubble up to the API
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ForbiddenAccessException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Body = await "{}".ToStreamAsync(cancellationToken);
            return true;
        }
        
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.Body = await "{}".ToStreamAsync(cancellationToken);
        return true;
    }
}