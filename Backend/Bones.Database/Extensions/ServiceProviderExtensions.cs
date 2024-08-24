using Bones.Database.Models;
using Bones.Database.Operations.Setup;
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
        DatabaseConfiguration config = scope.ServiceProvider.GetRequiredService<DatabaseConfiguration>();
        if (!config.UseInMemoryDb)
        {
            BonesDbContext db = scope.ServiceProvider.GetRequiredService<BonesDbContext>();
            if (db.Database.GetPendingMigrations().Any())
            {
                db.Database.Migrate();
            }
        }

        // We don't need to await this, it's fine to just execute in the background
        ISender sender = serviceProvider.GetRequiredService<ISender>();
        sender.Send(new SetupDbHandler.Command());
    }
}