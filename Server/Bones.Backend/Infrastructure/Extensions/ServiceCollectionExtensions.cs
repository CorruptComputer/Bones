using Bones.Backend.HostedServices.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Backend.Infrastructure.Extensions;

/// <summary>
///     Extensions for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the HostedServices used by the Bones Backend.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddBonesHostedServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddHostedService<TaskHostedService>();
    }
}