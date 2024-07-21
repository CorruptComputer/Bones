using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bones.Api;
using Bones.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Bones.Testing.Shared;

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
        hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        hostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
        {
            builder.RegisterModule<BonesDatabaseModule>();
            builder.RegisterModule<BonesApiModule>();
            builder.RegisterModule<UnitTestModule>();
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddSerilog((serviceProvider, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(serviceProvider)
            );

            services.AddDbContext<BonesDbContext>();
        });

        return hostBuilder.Build();
    }
}