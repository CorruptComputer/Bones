using Bones.Backend.Tasks.Minutely;
using Bones.Backend.Tasks.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Backend.Extensions;

/// <summary>
///   Extensions for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///   Registers the background tasks for the Backend
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterBackgroundTasks(this IServiceCollection services)
    {
        services.AddHostedService<SetupBackgroundTaskUserTask>();
        services.AddHostedService<SendConfirmationEmailTask>();
    }
}