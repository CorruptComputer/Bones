using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Bones.Shared.Backend.Extensions;

/// <summary>
///   Extensions for the WebApplicationBuilder class
/// </summary>
public static class IHostApplicationBuilderExtensions
{
    /// <summary>
    ///   Adds the required services for the Aspire framework
    /// </summary>
    /// <param name="builder"></param>
    public static void AddAspire(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                // We want to view all traces
                tracing.SetSampler(new AlwaysOnSampler());

                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });


        bool useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        if (useOtlpExporter)
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(
                logging => logging.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryMeterProvider(
                metrics => metrics.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryTracerProvider(
                tracing => tracing.AddOtlpExporter());
        }

        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });
    }
}
