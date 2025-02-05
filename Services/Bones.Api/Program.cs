using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Api.Handlers;
using Bones.Api.Models;
using Bones.Logic;
using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Extensions;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Bones.Api;

/// <summary>
///     Should be self-explanatory as to what this is.
/// </summary>
public static class Program
{
    /// <summary>
    ///     The main character of the project.
    /// </summary>
    /// <param name="args">Arg, I'm a pirate.</param>
    public static async Task Main(string[] args)
    {
        await WebApplication.CreateBuilder(args).BuildBonesApi().RunBonesApiAsync();
    }

    private static WebApplication BuildBonesApi(this WebApplicationBuilder builder)
    {
        // Aspire
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
                if (builder.Environment.IsDevelopment())
                {
                    // We want to view all traces in development
                    tracing.SetSampler(new AlwaysOnSampler());
                }

                tracing.AddAspNetCoreInstrumentation()
                    // Uncomment the following line to enable gRPC instrumentation 
                    // (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
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
        // End Aspire

        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule(new BonesApiModule(builder.Configuration,
                [typeof(BonesBackendModule).Assembly, typeof(BonesDatabaseModule).Assembly]));
            containerBuilder.RegisterModule(new BonesBackendModule(builder.Configuration, builder.Services));
            containerBuilder.RegisterModule(new BonesDatabaseModule(builder.Configuration, builder.Services));
        });

        builder.WebHost.UseKestrel().ConfigureKestrel(kestrelServerOptions =>
        {
            kestrelServerOptions.AddServerHeader = false;
        });

        builder.Services.AddExceptionHandler<ApiExceptionHandler>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicy.SYSTEM_ADMINISTRATOR, policy =>
            {
                policy.RequireClaim(BonesClaimTypes.Role.System.SYSTEM_ADMINISTRATOR, ClaimValues.YES);
            });

        builder.Services.AddIdentity<BonesUser, BonesRole>(options => options.AddBonesIdentityOptions())
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddRoles<BonesRole>()
            .AddEntityFrameworkStores<BonesDbContext>();

        builder.Services.AddControllers().AddJsonOptions(configure =>
        {
            configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("develop", new()
            {
                Version = "develop",
                Title = "Bones API",
                Description = "Its an API and it does stuff",
                Contact = new()
                {
                    Name = "GitHub Issues",
                    Url = new("https://github.com/CorruptComputer/Bones/issues")
                },
                License = new()
                {
                    Name = "MIT License",
                    Url = new("https://github.com/CorruptComputer/Bones/blob/develop/LICENSE")
                }
            });

            string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddSerilog((serviceProvider, loggerConfig) =>
            loggerConfig
                .ReadFrom.Services(serviceProvider)
                .ReadFrom.Configuration(builder.Configuration)
        );

        builder.Services.AddDbContext<BonesDbContext>();

        return builder.Build();
    }

    private static async Task RunBonesApiAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        ApiConfiguration apiConfig = scope.ServiceProvider.GetRequiredService<ApiConfiguration>();
        string[] corsAllowedOrigins = apiConfig.CorsAllowedOrigins
            ?? throw new BonesException("ApiConfiguration:CorsAllowedOrigins missing from appsettings.");

        Log.Information("Environment: {Environment}\nAllowed origins: {Origins}",
            app.Environment.EnvironmentName,
            string.Join(" | ", corsAllowedOrigins));

        app.UseCors(configurePolicy =>
        {
            configurePolicy
                .WithOrigins(corsAllowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseReDoc(c =>
            {
                c.DocumentTitle = "Bones API Documentation";
                c.SpecUrl = "/swagger/develop/swagger.json";
            });

            // Really clutters up the logs, but is useful sometimes
            // app.UseSerilogRequestLogging()
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Aspire
        // All health checks must pass for app to be considered ready to 
        // accept traffic after starting
        app.MapHealthChecks("/health");

        // Only health checks tagged with the "live" tag must pass for 
        // app to be considered alive
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });
        // End Aspire

        Log.Information("Startup complete");
        await app.RunAsync();
    }
}