using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Extensions;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Bones.Api;

internal static class Setup
{
    internal static void AddApiAuthenticationAndAuthorization(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                // Does this by default, had to copy the entire AddIdentity<TUser, TRole>() method just to change this shit
                //o.LoginPath = new PathString("/Account/Login")
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync,
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }
                };
            }).AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            }).AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
                };
            }).AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                o.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToReturnUrl = _ => Task.CompletedTask
                };
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });

        // Hosting doesn't add IHttpContextAccessor by default
        services.AddHttpContextAccessor();
        // Identity services
        services.TryAddScoped<IUserValidator<BonesUser>, UserValidator<BonesUser>>();
        services.TryAddScoped<IPasswordValidator<BonesUser>, PasswordValidator<BonesUser>>();
        services.TryAddScoped<IPasswordHasher<BonesUser>, PasswordHasher<BonesUser>>();
        services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
        services.TryAddScoped<IRoleValidator<BonesRole>, RoleValidator<BonesRole>>();
        // No interface for the error describer so we can add errors without rev'ing the interface
        services.TryAddScoped<IdentityErrorDescriber>();
        services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<BonesUser>>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<SecurityStampValidatorOptions>, PostConfigureSecurityStampValidatorOptions>());
        services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<BonesUser>>();
        services.TryAddScoped<IUserClaimsPrincipalFactory<BonesUser>, UserClaimsPrincipalFactory<BonesUser, BonesRole>>();
        services.TryAddScoped<IUserConfirmation<BonesUser>, DefaultUserConfirmation<BonesUser>>();
        services.TryAddScoped<UserManager<BonesUser>>();
        services.TryAddScoped<SignInManager<BonesUser>>();
        services.TryAddScoped<RoleManager<BonesRole>>();

        services.Configure((Action<IdentityOptions>)(options => options.AddBonesIdentityOptions()));

        IdentityBuilder idBuilder = new(typeof(BonesUser), typeof(BonesRole), services);

        idBuilder.AddDefaultTokenProviders()
            .AddEntityFrameworkStores<BonesDbContext>();

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicy.SYSTEM_ADMINISTRATOR, policy =>
            {
                policy.RequireClaim(BonesClaimTypes.Role.System.SYSTEM_ADMINISTRATOR, ClaimValues.YES);
            });
    }

    internal static void AddAspire(this WebApplicationBuilder builder)
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
    }

    internal static void UseAspire(this WebApplication app)
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

internal sealed class PostConfigureSecurityStampValidatorOptions : IPostConfigureOptions<SecurityStampValidatorOptions>
{
    public PostConfigureSecurityStampValidatorOptions(TimeProvider? timeProvider = null)
    {
        // We could assign this to "timeProvider ?? TimeProvider.System", but
        // SecurityStampValidator already has system clock fallback logic.
        TimeProvider = timeProvider;
    }

    private TimeProvider? TimeProvider { get; }

    public void PostConfigure(string? name, SecurityStampValidatorOptions options)
    {
        options.TimeProvider ??= TimeProvider;
    }
}