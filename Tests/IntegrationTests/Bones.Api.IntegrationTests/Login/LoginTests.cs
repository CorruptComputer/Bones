namespace Bones.Api.IntegrationTests.Login;

/// <summary>
///   Provides tests for the login functionality of the Bones API.
/// </summary>
public class LoginTests : TestBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DefaultAdmin_ShouldBeAbleToLogin()
    {
        await SetupApiClientAsync();

        if (ApiClient is not null)
        {
            (string email, string password) = TestCredentials.Credentials[TestCredentials.User.DefaultAdmin];

            Task<EmptyResponse> act = ApiClient.LoginAsync(new()
            {
                Email = email,
                Password = password
            });

            await act.ShouldNotThrowAsync();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Invalid_ShouldNotBeAbleToLogin()
    {
        await SetupApiClientAsync();

        if (ApiClient is not null)
        {
            (string email, string password) = TestCredentials.Credentials[TestCredentials.User.Invalid];

            Task<EmptyResponse> act = ApiClient.LoginAsync(new()
            {
                Email = email,
                Password = password
            });

            await act.ShouldThrowAsync<ApiException<EmptyResponse>>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Unconfirmed_ShouldNotBeAbleToLogin()
    {
        await SetupApiClientAsync();

        if (ApiClient is not null)
        {
            (string email, string password) = TestCredentials.Credentials[TestCredentials.User.Unconfirmed];

            await ApiClient.RegisterAsync(new()
            {
                Email = email,
                Password = password
            });

            Task<EmptyResponse> act = ApiClient.LoginAsync(new()
            {
                Email = email,
                Password = password
            });

            await act.ShouldThrowAsync<ApiException<EmptyResponse>>();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RepeatedCallsToLogout_ShouldNotError()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        if (ApiClient is not null)
        {
            Task<EmptyResponse> act = ApiClient.LogoutAsync();
            await act.ShouldNotThrowAsync();

            Task<EmptyResponse> act2 = ApiClient.LogoutAsync();
            await act2.ShouldNotThrowAsync();

            Task<EmptyResponse> act3 = ApiClient.LogoutAsync();
            await act3.ShouldNotThrowAsync();
        }
    }
}
