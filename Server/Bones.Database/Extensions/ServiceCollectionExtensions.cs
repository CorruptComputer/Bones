using Microsoft.Extensions.DependencyInjection;

namespace Bones.Database.Extensions;

/// <summary>
///     Extensions for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the DbContext to the DI registry
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddBonesDbContext(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddDbContext<BonesDbContext>();
    }
}