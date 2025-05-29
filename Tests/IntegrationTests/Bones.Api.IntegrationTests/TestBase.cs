namespace Bones.Api.IntegrationTests;

/// <summary>
///   Base class for integration tests.
/// </summary>
public abstract class TestBase
{
    private ApiService? ApiService { get; set; }

    internal BonesApiClient? ApiClient => ApiService?.Client;

    internal async Task SetupApiClientAsync(CancellationToken cancellationToken = default)
    {
        ApiService = await ApiService.CreateAsync(cancellationToken);
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
