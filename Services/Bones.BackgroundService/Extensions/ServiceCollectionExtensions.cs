using System.Reflection;
using Bones.BackgroundService.Tasks.Minutely;
using Bones.BackgroundService.Tasks.Startup;
using Bones.Shared.Exceptions;

namespace Bones.BackgroundService.Extensions;

/// <summary>
///   Extensions for IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///   Registers the background tasks for the Backend
    /// </summary>
    /// <remarks>
    ///   Don't call AddHostedService directly in these, instead use RegisterHostedService so the scope isn't fucked
    /// </remarks>
    /// <param name="services"></param>
    public static void RegisterBackgroundTasks(this IServiceCollection services)
    {
        services.RegisterStartupTasks();
        services.RegisterMinutelyTasks();
    }

    private static void RegisterStartupTasks(this IServiceCollection services)
    {
        services.RegisterHostedService<SetupDatabase>();
    }

    private static void RegisterMinutelyTasks(this IServiceCollection services)
    {
        services.RegisterHostedService<SendConfirmationEmailTask>();
        services.RegisterHostedService<SendForgotPasswordEmailTask>();
    }

    // By default it does singleton scope, which kinda sucks since all of our services end up using the same db context
    // EF throws if multiple threads try to use the same context at the same time, need to scope to each hosted service
    private static void RegisterHostedService<T>(this IServiceCollection services)
        where T : class, IHostedService
    {
        services.AddHostedService(serviceProvider =>
        {
            IServiceScope scope = serviceProvider.CreateScope();
            ConstructorInfo ctor = typeof(T).GetConstructors()[0];
            ParameterInfo[] parameters = ctor.GetParameters();

            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = scope.ServiceProvider.GetRequiredService(parameters[i].ParameterType);
            }

            return Activator.CreateInstance(typeof(T), args) as T
                ?? throw new BonesException("Failed to create hosted service");
        });
    }
}