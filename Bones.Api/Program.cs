using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Database;
using Bones.Database.DbSets.Identity;
using Bones.Database.Extensions;

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
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
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
            app.UseSwagger();
            app.UseSwaggerUI();

            // Really clutters up the logs, but is useful sometimes
            //app.UseSerilogRequestLogging();
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.MapControllers();
        app.Run();
    }
}