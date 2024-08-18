using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Database;
using Bones.Database.DbSets.Identity;
using Bones.Database.Extensions;
using Bones.Shared.Exceptions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

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
        builder.Services.AddAuthentication().AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Jwt:Authority"];
            options.Configuration = new()
            {
                Issuer = builder.Configuration["Jwt:Issuer"],
                SigningKeys = { 
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] 
                        ?? throw new BonesException("Missing configuration: Jwt:Key"))) 
                }
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
                options.SwaggerDoc("v0", new()
                {
                    Version = "v0",
                    Title = "Bones API",
                    Description = "Its an API and it does stuff",
                    TermsOfService = new Uri("https://example.com/terms"),
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
        builder.Services.AddIdentityApiEndpoints<BonesUser>()
            .AddEntityFrameworkStores<BonesDbContext>();

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
                options.SwaggerEndpoint("/swagger/v0/swagger.json", "v0");
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