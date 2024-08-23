using Bones.Database.Operations.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Database.Extensions;

/// <summary>
///     Extensions for IServiceProvider
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    ///     Migrates the DB up to the latest version
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void MigrateBonesDb(this IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        string useInMemoryDbStr = config["Database:UseInMemoryDb"] ?? "false";
        bool useInMemoryDb = bool.Parse(useInMemoryDbStr);
        if (!useInMemoryDb)
        {
            BonesDbContext db = scope.ServiceProvider.GetRequiredService<BonesDbContext>();
            db.Database.Migrate();
        }

        // We don't need to await this, it's fine to just execute in the background
        ISender sender = serviceProvider.GetRequiredService<ISender>();
        sender.Send(new SetupDbHandler.Command());
    }
}