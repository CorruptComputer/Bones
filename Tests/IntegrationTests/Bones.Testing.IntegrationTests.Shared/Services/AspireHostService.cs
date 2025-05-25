using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Bones.AppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Testing.IntegrationTests.Shared.Services;

/// <summary>
///   Provides a singleton service for managing the Aspire host.
/// </summary>
public class AspireHostService : IAsyncDisposable
{
    private static AspireHostService? _instance;

    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly DistributedApplication _application;

    private AspireHostService(DistributedApplication app)
    {
        _application = app;
    }

    /// <summary>
    ///   Creates or retrieves the singleton instance of <see cref="AspireHostService"/>.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<AspireHostService> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_instance == null)
        {
            try
            {
                bool lockAcquired = false;

                do
                {
                    lockAcquired = await _semaphore.WaitAsync(TimeSpan.FromSeconds(1), cancellationToken);
                }
                while (!lockAcquired);

                _instance = await CreateAsync(cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return _instance;
    }

    /// <summary>
    ///   Gets an <see cref="HttpClient"/> configured to communicate with the API service.
    /// </summary>
    /// <returns></returns>
    public HttpClient GetApiHttpClient()
    {
        HttpClient client = _application.CreateHttpClient(ServiceNames.Api);
        return client;
    }

    private static async Task<AspireHostService> CreateAsync(CancellationToken cancellationToken)
    {
        IDistributedApplicationTestingBuilder builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Bones_AppHost>(cancellationToken);

        builder.Services.ConfigureHttpClientDefaults(client =>
        {
            client.AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(60);
                options.CircuitBreaker.FailureRatio = 1.0;
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(120);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(180);
            });
        });

        Environment.SetEnvironmentVariable("BonesBackendConfiguration__UseInMemoryDb", "true");
        Environment.SetEnvironmentVariable("BonesBackendConfiguration__InMemoryDbId", Guid.NewGuid().ToString());
        // Name is a bit misleading in this context, used to indicate if default data should be loaded for manual testing.
        // Anything needed in these tests can be created by the tests themselves.
        Environment.SetEnvironmentVariable("BonesBackendConfiguration__SetupForTesting", "false");

        Dictionary<string, string?> environmentVariables = new()
        {
            { "DcpPublisher:RandomizePorts", "false" }
        };

        builder.Configuration.AddInMemoryCollection(environmentVariables);

        DistributedApplication app = await builder.BuildAsync(cancellationToken);
        ResourceNotificationService resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

        await app.StartAsync(cancellationToken);

        // No tests need the background service, we don't need to wait for it.
        //await resourceNotificationService.WaitForResourceAsync(ServiceNames.BackgroundService, KnownResourceStates.Running, cancellationToken);
        await resourceNotificationService.WaitForResourceAsync(ServiceNames.Api, KnownResourceStates.Running, cancellationToken);
        // There might be some way to automate this, but for now we can just skip waiting for it.
        //await resourceNotificationService.WaitForResourceAsync(ServiceNames.WebUI, KnownResourceStates.Running, cancellationToken);

        return new(app);
    }

    #region IAsyncDisposable
    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await _application.StopAsync();
        await _application.DisposeAsync();
        _instance = null;

        GC.SuppressFinalize(this);
    }
    #endregion
}
