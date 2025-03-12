using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Bones.Shared.Backend.Extensions;

/// <summary>
///   Extensions for the WebApplication class
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    ///   Use the Aspire health checks
    /// </summary>
    /// <param name="app"></param>
    public static void UseAspire(this WebApplication app)
    {
        // All health checks must pass for app to be considered ready to 
        // accept traffic after starting
        app.MapHealthChecks("/health");

        // Only health checks tagged with the "live" tag must pass for 
        // app to be considered alive
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });
    }
}
