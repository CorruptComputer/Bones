using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Logic;
using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Bones.Database.Operations.System;
using Bones.Shared.Backend.Models;

namespace Bones.Testing.Shared.Backend;

internal static class TestFactory
{
    internal static ISender GetTestSender()
    {
        IServiceProvider provider = GetTestServiceProvider();

        ClearInMemoryDb(provider.GetRequiredService<BonesDbContext>());

        ISender sender = provider.GetRequiredService<ISender>();

        sender.Send(new SetupDb.Command()).Wait();

        return sender;
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
        });

        BonesBackendConfiguration config = new()
        {
            UseInMemoryDb = true
        };

        hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddSerilog((serviceProvider, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(serviceProvider)
            );

            hostBuilder.ConfigureContainer<ContainerBuilder>((containerCtx, containerBuilder) =>
            {
                containerBuilder.RegisterModule(new BonesBackendModule(services));
                containerBuilder.RegisterModule(new BonesDatabaseModule(services));
                containerBuilder.RegisterModule(new UnitTestModule([typeof(BonesBackendModule).Assembly, typeof(BonesDatabaseModule).Assembly]));
            });

            services.AddIdentity<BonesUser, BonesRole>(options => options.AddBonesIdentityOptions())
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddRoles<BonesRole>()
                .AddEntityFrameworkStores<BonesDbContext>();

            services.AddSingleton(config);

            services.AddDbContext<BonesDbContext>();
        });

        return hostBuilder.Build();
    }
}