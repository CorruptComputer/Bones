using System.Net;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Api.Models;
using Bones.Backend;
using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Extensions;
using Bones.Shared.Backend.Consts;
using Bones.Shared.Backend.Extensions;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;

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
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule(new BonesApiModule(builder.Configuration));
            containerBuilder.RegisterModule(new BonesBackendModule(builder.Configuration));
            containerBuilder.RegisterModule(new BonesDatabaseModule(builder.Configuration));
        });

        // In Development this is set by Properties/launchSettings.json
        // so only need to include this for Production.
        if (!builder.Environment.IsDevelopment())
        {
            builder.WebHost.UseKestrel().ConfigureKestrel(kestrelServerOptions =>
            {
                kestrelServerOptions.AddServerHeader = false;
                kestrelServerOptions.Listen(IPAddress.Any, 8443, options =>
                {
                    options.UseHttps();
                });
            });
        }

        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax; // I don't feel like dealing with CSRF Tokens right now

            options.Events.OnRedirectToAccessDenied = (context) => throw new AuthenticationFailedException(true, "Forbidden");
            options.Events.OnRedirectToLogin = (context) => throw new AuthenticationFailedException(false, "Unauthorized");
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicy.SYSTEM_ADMINISTRATOR, policy =>
            {
                policy.RequireRole(SystemRoles.SYSTEM_ADMINISTRATORS);
                policy.RequireClaim(ClaimTypes.Role.System.SYSTEM_ADMINISTRATOR, ClaimValues.YES);
            });
        });

        builder.Services.AddControllers().AddJsonOptions(configure =>
        {
            configure.JsonSerializerOptions.WriteIndented = true;
            configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddIdentityCore<BonesUser>(options => options.OverwriteWith(Identity.IdentityOptions))
            .AddRoles<BonesRole>().AddEntityFrameworkStores<BonesDbContext>();

        builder.Services.AddHttpContextAccessor();

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
        });

        builder.Services.AddSerilog((serviceProvider, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(serviceProvider)
        );

        builder.Services.AddHostedService<BackgroundTaskScheduler>();
        builder.Services.AddDbContext<BonesDbContext>();



        WebApplication app = builder.Build();
        app.Services.MigrateBonesDb();

        using IServiceScope scope = app.Services.CreateScope();
        ApiConfiguration apiConfig = scope.ServiceProvider.GetRequiredService<ApiConfiguration>();
        
        app.UseCors(configurePolicy =>
        {
            configurePolicy
                .WithOrigins(apiConfig.WebUIBaseUrl ?? throw new BonesException("ApiConfiguration:WebUIBaseUrl missing from appsettings."))
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

        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}