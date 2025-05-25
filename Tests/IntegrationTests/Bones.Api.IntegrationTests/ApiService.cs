using Bones.Testing.IntegrationTests.Shared.Services;

namespace Bones.Api.IntegrationTests;

/// <summary>
///   Provides access to the Bones API client, this is not a singleton. Each test should create its own instance.
/// </summary>
public class ApiService : IAsyncDisposable
{
    /// <summary>
    ///   The client for the API.
    /// </summary>
    public BonesApiClient Client { get; }

    private ApiService(BonesApiClient client)
    {
        Client = client;
    }

    internal static async Task<ApiService> CreateAsync(CancellationToken cancellationToken = default)
    {
        AspireHostService hostService = await AspireHostService.GetAsync(cancellationToken);
        return new ApiService(new(hostService.GetApiHttpClient()));
    }

    #region IAsyncDisposable
    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
    #endregion
}
