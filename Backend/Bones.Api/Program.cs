using System.Reflection;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Api.Handlers;
using Bones.Api.Models;
using Bones.Backend;
using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
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
        WebApplication.CreateBuilder(args).BuildBonesApi().RunBonesApi();
    }

    private static WebApplication BuildBonesApi(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule(new BonesApiModule(builder.Configuration));
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
            loggerConfig.ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(serviceProvider)
        );

        builder.Services.AddDbContext<BonesDbContext>();

        return builder.Build();
    }

    private static void RunBonesApi(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        ApiConfiguration apiConfig = scope.ServiceProvider.GetRequiredService<ApiConfiguration>();

        app.UseCors(configurePolicy =>
        {
            configurePolicy
                .WithOrigins(
                    apiConfig.WebUIBaseUrl ?? throw new BonesException("ApiConfiguration:WebUIBaseUrl missing from appsettings."),
                    apiConfig.ApiBaseUrl ?? throw new BonesException("ApiConfiguration:ApiBaseUrl missing from appsettings."))
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

        app.Run();
    }
}