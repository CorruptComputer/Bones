using Bones.BackgroundService.Tasks.Minutely;
using Bones.BackgroundService.Tasks.Startup;

namespace Bones.BackgroundService.Extensions;

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
        services.RegisterStartupTasks();
        services.RegisterMinutelyTasks();
    }
    
    private static void RegisterStartupTasks(this IServiceCollection services)
    {
        services.AddHostedService<SetupBackgroundTaskUserTask>();
    }

    private static void RegisterMinutelyTasks(this IServiceCollection services)
    {
        services.AddHostedService<SendConfirmationEmailTask>();
    }
}