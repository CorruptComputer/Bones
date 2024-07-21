using Bones.Api.HostedServices.Tasks;

namespace Bones.Api.Extensions;

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