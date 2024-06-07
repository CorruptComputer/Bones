using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Backend;
using Bones.Backend.Infrastructure.Extensions;
using Bones.Database;
using Bones.Database.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Bones.Api;

/// <summary>
///   Should be self-explanatory as to what this is.
/// </summary>
public static class Program
{
    /// <summary>
    ///   The main character of the project.
    /// </summary>
    /// <param name="args">Arg, I'm a pirate.</param>
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddOpenApi("BonesApi")
        builder.Services.AddSwaggerGen();

        builder.Services.AddSerilog((serviceProvider, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(builder.Configuration)
                        .ReadFrom.Services(serviceProvider)
        );

        builder.Services.AddBonesDbContext();

        AuthenticationBuilder authBuilder = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        builder.Services.AddBonesHostedServices();

        authBuilder.AddJwtBearer(jwtOptions =>
        {
            string? jwtIssuer = builder.Configuration["Jwt:Issuer"];
            string? jwtAudience = builder.Configuration["Jwt:Audience"];
            string? jwtKey = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtIssuer)
                || string.IsNullOrWhiteSpace(jwtAudience)
                || string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new BonesException("Appsettings missing JWT Issuer, Audience, or Key.");
            }

            jwtOptions.TokenValidationParameters = new()
            {
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule<BonesBackendModule>();
            containerBuilder.RegisterModule<BonesDatabaseModule>();
        });

        WebApplication app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            //app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Services.MigrateBonesDb();
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}