using Bones.Database.Operations.System;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Database.Extensions;

/// <summary>
///   Extensions for IServiceProvider
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    ///   Sets up the database, migrating if needed and setting up initial settings
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    public static async Task MigrateDatabase(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new MigrateDb.Command(), cancellationToken);
    }
}