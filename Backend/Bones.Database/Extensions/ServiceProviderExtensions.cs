using Bones.Database.Operations.Setup.SetupDb;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Database.Extensions;

/// <summary>
///     Extensions for IServiceProvider
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    ///     Sets up the database, migrating if needed and setting up initial settings
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    public static async Task SetupDatabase(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new SetupDbCommand(), cancellationToken);
    }
}