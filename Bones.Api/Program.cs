using System.Text.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Api.Handlers;
using Bones.Database;
using Bones.Database.DbSets.Identity;
using Bones.Database.Extensions;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;


namespace Bones.Api;

/// <summary>
///     Should be self-explanatory as to what this is.
/// </summary>
public static class Program
{
    // Because fuck you Microsoft IdentityConstants.BearerAndApplicationScheme is internal only for some probably dumb reason
    private const string BearerAndApplicationScheme = "Identity.BearerAndApplication";
    
    /// <summary>
    ///     The main character of the project.
    /// </summary>
    /// <param name="args">Arg, I'm a pirate.</param>
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthentication(BearerAndApplicationScheme)
            .AddScheme<AuthenticationSchemeOptions, BonesIdentityHandler>
                (BearerAndApplicationScheme, null, compositeOptions =>
            {
                compositeOptions.ForwardDefault = IdentityConstants.BearerScheme;
                compositeOptions.ForwardAuthenticate = BearerAndApplicationScheme;
            })
            .AddBearerToken(IdentityConstants.BearerScheme)
            .AddIdentityCookies();
        
        builder.Services.AddIdentityCore<BonesUser>(options =>
        {
            options.User = new()
            {
                RequireUniqueEmail = true
            };

            options.SignIn = new()
            {
                RequireConfirmedEmail = true
            };

            options.Lockout = new()
            {
                MaxFailedAccessAttempts = 3,
                DefaultLockoutTimeSpan = TimeSpan.FromHours(1),
                AllowedForNewUsers = true
            };

            options.Password = new()
            {
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true,
                RequiredLength = 8
            };
        });
        
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        // builder.Services.AddOpenApi()

        if (builder.Environment.IsDevelopment())
        {
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
        }

        builder.Services.AddSerilog((serviceProvider, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(serviceProvider)
        );

        builder.Services.AddDbContext<BonesDbContext>();
        
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule<BonesApiModule>();
            containerBuilder.RegisterModule<BonesDatabaseModule>();
        });

        WebApplication app = builder.Build();
        app.Services.MigrateBonesDb();
        
        if (app.Environment.IsDevelopment())
        {
            // app.MapOpenApi()
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/develop/swagger.json", "develop");
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
        
        string frontEndUrl = app.Configuration["WebUIBaseUrl"] ?? throw new BonesException("WebUIBaseUrl missing from appsettings.");
        app.UseCors(configurePolicy =>
        {
            configurePolicy.WithOrigins(frontEndUrl)
                .AllowAnyMethod()
                .AllowAnyHeader();
        }); 
        app.Run();
    }
}