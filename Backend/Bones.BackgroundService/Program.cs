using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Backend;
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
        using CancellationTokenSource cts = new();
        await Host.CreateApplicationBuilder(args).BuildBonesBackgroundService().RunBonesBackgroundService(cts.Token);
    }

    private static IHost BuildBonesBackgroundService(this HostApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables();

        builder.ConfigureContainer(new AutofacServiceProviderFactory(), containerBuilder =>
        {
            containerBuilder.RegisterModule(new BonesBackgroundServiceModule(builder.Configuration));
            containerBuilder.RegisterModule(new BonesBackendModule(builder.Configuration));
            containerBuilder.RegisterModule(new BonesDatabaseModule(builder.Configuration));
        });

        builder.Services.AddIdentityCore<BonesUser>(options => options.AddBonesIdentityOptions())
            .AddRoles<BonesRole>()
            .AddEntityFrameworkStores<BonesDbContext>();


        builder.Services.AddSerilog((serviceProvider, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(serviceProvider)
        );

        builder.Services.RegisterBackgroundTasks();
        builder.Services.AddDbContext<BonesDbContext>();

        return builder.Build();
    }

    private static async Task RunBonesBackgroundService(this IHost host, CancellationToken cancellationToken)
    {
        await host.Services.SetupDatabase(cancellationToken);
        await host.RunAsync(cancellationToken);
    }
}