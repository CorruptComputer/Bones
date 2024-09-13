using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Api;
using Bones.Backend;
using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Bones.Testing.Shared.Backend;

internal static class TestFactory
{
    internal static ISender GetTestSender()
    {
        IServiceProvider provider = GetTestServiceProvider();

        ClearInMemoryDb(provider.GetRequiredService<BonesDbContext>());

        return provider.GetRequiredService<ISender>();
    }

    private static void ClearInMemoryDb(BonesDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    private static IServiceProvider GetTestServiceProvider()
    {
        IHost host = CreateTestHost();
        return host.Services.CreateScope().ServiceProvider;
    }

    private static IHost CreateTestHost()
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();
        hostBuilder.ConfigureAppConfiguration(configBuilder =>
        {
            // Want to make sure we aren't picking up any appsettings.json files
            configBuilder.Sources.Clear();

            configBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
            {
                //new("ApiConfiguration:WebUIBaseUrl", "http://localhost:9080"),
                //new("BackgroundService:BackgroundTasksUserEmail", string.Empty),
                new("BackendConfiguration:WebUIBaseUrl", "http://localhost:9080"),
                new("DatabaseConfiguration:ConnectionString", string.Empty),
                new("DatabaseConfiguration:UseInMemoryDb", "true"),
            });
        });
        
        hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddSerilog((serviceProvider, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(serviceProvider)
            );
            
            hostBuilder.ConfigureContainer<ContainerBuilder>((containerCtx, containerBuilder) =>
            {
                //containerBuilder.RegisterModule(new BonesApiModule(containerCtx.Configuration));
                containerBuilder.RegisterModule(new BonesBackendModule(containerCtx.Configuration, services));
                containerBuilder.RegisterModule(new BonesDatabaseModule(containerCtx.Configuration, services));
                containerBuilder.RegisterModule<UnitTestModule>();
            });

            services.AddIdentity<BonesUser, BonesRole>(options => options.AddBonesIdentityOptions())
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddRoles<BonesRole>()
                .AddEntityFrameworkStores<BonesDbContext>();

            services.AddDbContext<BonesDbContext>();
        });

        return hostBuilder.Build();
    }
}