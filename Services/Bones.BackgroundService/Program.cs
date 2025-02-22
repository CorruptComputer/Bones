using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Logic;
using Bones.BackgroundService.Extensions;
using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Extensions;
using Bones.Shared.Backend.Extensions;

namespace Bones.BackgroundService;

/// <summary>
///   Idk its a thing and it does stuff
/// </summary>
public static class Program
{
    /// <summary>
    ///   Gets it going
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        await Host.CreateApplicationBuilder(args).BuildBonesBackgroundService().RunBonesBackgroundService();
    }

    private static IHost BuildBonesBackgroundService(this HostApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables();

        builder.ConfigureContainer(new AutofacServiceProviderFactory(), containerBuilder =>
        {
            containerBuilder.RegisterModule(new BonesBackgroundServiceModule([typeof(BonesBackendModule).Assembly, typeof(BonesDatabaseModule).Assembly]));

            containerBuilder.RegisterModule(new BonesBackendModule(builder.Services));
            containerBuilder.RegisterModule(new BonesDatabaseModule(builder.Configuration, builder.Services));
        });

        builder.Services.AddIdentityCore<BonesUser>(options => options.AddBonesIdentityOptions())
            .AddRoles<BonesRole>()
            .AddEntityFrameworkStores<BonesDbContext>();

        builder.Services.AddSerilog((serviceProvider, loggerConfig) =>
            loggerConfig
                .ReadFrom.Services(serviceProvider)
                .ReadFrom.Configuration(builder.Configuration)
        );

        builder.Services.RegisterBackgroundTasks();
        builder.Services.AddDbContext<BonesDbContext>();

        return builder.Build();
    }

    private static async Task RunBonesBackgroundService(this IHost host)
    {
        await host.Services.MigrateDatabase();

        Log.Information("Startup complete");
        await host.RunAsync();
    }
}