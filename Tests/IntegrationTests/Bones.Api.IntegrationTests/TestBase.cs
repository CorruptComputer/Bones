namespace Bones.Api.IntegrationTests;

/// <summary>
///   Base class for integration tests.
/// </summary>
public abstract class TestBase
{
    private ApiService? _apiService { get; set; }

    internal BonesApiClient? ApiClient => _apiService?.Client;

    internal async Task SetupApiClientAsync(CancellationToken cancellationToken = default)
    {
        _apiService = await ApiService.CreateAsync(cancellationToken);
    }

    internal async Task LoginAsync(TestCredentials.User user, CancellationToken cancellationToken = default)
    {
        if (ApiClient is not null)
        {
            (string email, string password) = TestCredentials.Credentials[user];

            await ApiClient.LoginAsync(new()
            {
                Email = email,
                Password = password
            }, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("ApiClient is not initialized. Call SetupApiClientAsync first.");
        }
    }
    
    internal async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (ApiClient is not null)
        {
            await ApiClient.LogoutAsync(cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("ApiClient is not initialized. Call SetupApiClientAsync first.");
        }
    }
}
