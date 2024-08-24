using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Extensions;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;


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
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax; // I don't feel like dealing with CSRF Tokens right now

            options.Events.OnRedirectToAccessDenied = (context) => throw new AuthenticationFailedException(true, "Forbidden");
            options.Events.OnRedirectToLogin = (context) => throw new AuthenticationFailedException(false, "Unauthorized");
        });

        builder.Services.AddAuthorization();
        builder.Services.AddControllers().AddJsonOptions(configure =>
        {
            configure.JsonSerializerOptions.WriteIndented = true;
            configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddIdentityApiEndpoints<BonesUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
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
        }).AddRoles<BonesRole>().AddEntityFrameworkStores<BonesDbContext>();

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

        string frontEndUrl = app.Configuration["WebUIBaseUrl"] ?? throw new BonesException("WebUIBaseUrl missing from appsettings.");
        app.UseCors(configurePolicy =>
        {
            configurePolicy.WithOrigins(frontEndUrl)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });

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
        app.UseRouting().UseEndpoints(configure =>
        {
            configure.MapGroup("Auth").WithTags("Auth").MapIdentityApi<BonesUser>();
            configure.MapControllers();
        });



        app.Run();
    }
}