using Bones.Database;
using Bones.Database.DbSets.Accounts;
using Bones.Shared.Backend.Extensions;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bones.Api;

internal static class Setup
{
    internal static void AddApiAuthenticationAndAuthorization(this IServiceCollection services)
    {
        CookieAuthenticationEvents defaultCookieEvents = new()
        {
            OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync,
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                context.Response.WriteAsJsonAsync(new ErrorResponse(errorMessage: "You are not allowed to access this resource."));

                return Task.CompletedTask;
            },
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                context.Response.WriteAsJsonAsync(new ErrorResponse(errorMessage: "You are not authenticated."));

                return Task.CompletedTask;
            }
        };

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.Events = defaultCookieEvents;

            }).AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            }).AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                o.Events = defaultCookieEvents;
                o.Events.OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>;
            }).AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                o.Events = defaultCookieEvents;
                o.Events.OnRedirectToReturnUrl = _ => Task.CompletedTask;
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