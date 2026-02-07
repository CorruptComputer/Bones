using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Api.Handlers;
using Bones.Logic;
using Bones.Database;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using System.Text.Json;
using System.Reflection;
using Bones.Shared.Backend.Extensions;
using Bones.Database.Operations.System;
using Bones.Logic.Features.System.TestingDataSetup;

namespace Bones.Api;

/// <summary>
///   Should be self-explanatory as to what this is.
/// </summary>
public static class Program
{
    private static BonesBackendConfiguration? config = null;

    /// <summary>
    ///   The main character of the project.
    /// </summary>
    /// <param name="args">Arg, I'm a pirate.</param>
    public static async Task Main(string[] args)
    {
        await WebApplication.CreateBuilder(args).BuildBonesApi().RunBonesApiAsync();
    }

    private static WebApplication BuildBonesApi(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.AddAspire();
        }

        builder.Services.AddBonesGlobalSerilogConfiguration();
        config = builder.Configuration.AddBonesBackendConfiguration(builder.Environment);
        builder.Services.AddSingleton(config);

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule(new BonesApiModule([typeof(BonesBackendModule).Assembly, typeof(BonesDatabaseModule).Assembly]));
            containerBuilder.RegisterModule(new BonesBackendModule(builder.Services));
            containerBuilder.RegisterModule(new BonesDatabaseModule(builder.Services));
        });

        builder.WebHost.UseKestrel().ConfigureKestrel(kestrelServerOptions =>
        {
            kestrelServerOptions.AddServerHeader = false;
        });

        builder.Services.AddExceptionHandler<ApiExceptionHandler>();

        builder.Services.AddApiAuthenticationAndAuthorization();

        builder.Services.AddControllers().AddJsonOptions(configure =>
        {
            configure.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            configure.JsonSerializerOptions.WriteIndented = true;
            configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            configure.JsonSerializerOptions.AllowTrailingCommas = true;
            configure.JsonSerializerOptions.RespectNullableAnnotations = true;
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
            options.SupportNonNullableReferenceTypes();
            options.NonNullableReferenceTypesAsRequired();
            options.UseOneOfForPolymorphism();
            options.UseAllOfForInheritance();
        });

        builder.Services.AddDbContext<BonesDbContext>();

        return builder.Build();
    }

    private static async Task RunBonesApiAsync(this WebApplication app)
    {
        if (config?.ApiOnly ?? false)
        {
            using IServiceScope scope = app.Services.CreateScope();
            ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
            await sender.Send(new SetupDb.Command());

            if (config.SetupForTesting)
            {
                await sender.Send(new SetupTestData.Command());
            }
        }

        string[] corsAllowedOrigins = config?.CorsAllowedOrigins
            ?? throw new BonesException("BonesBackendConfiguration:CorsAllowedOrigins missing from appsettings.");

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

        app.UseExceptionHandler(opt => { });

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

        if (app.Environment.IsDevelopment())
        {
            app.UseAspire();
        }

        Log.Information("Startup complete");
        await app.RunAsync();
    }
}