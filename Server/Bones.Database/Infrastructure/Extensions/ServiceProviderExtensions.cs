using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Database.Infrastructure.Extensions;

/// <summary>
///   Extensions for IServiceProvider
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    ///   Migrates the DB up to the latest version
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void MigrateBonesDb(this IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        BonesDbContext db = scope.ServiceProvider.GetRequiredService<BonesDbContext>();
        db.Database.Migrate();
    }
}