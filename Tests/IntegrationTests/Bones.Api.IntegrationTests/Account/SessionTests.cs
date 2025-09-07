using ReQuesty.Runtime.Abstractions;

namespace Bones.Api.IntegrationTests.Account;

/// <summary>
///   Provides tests for the session functionality of the Bones API.
/// </summary>
public class SessionTests : TestBase
{
    /// <summary>
    ///   Default admin should be able to start a session
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DefaultAdmin_ShouldBeAbleToStartASession()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        GetOrCreateMySessionResponse? session = null;
        if (ApiClient is not null)
        {
            Task<GetOrCreateMySessionResponse?> task = ApiClient.Account.My.Session.GetAsync();
            await task.ShouldNotThrowAsync();
            session = await task;
        }

        session.ShouldNotBeNull();
        session.SessionId.ShouldNotBe(Guid.Empty);
        session.Base64LocalStorageKey.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    ///   Unauthenticated users should not be able to start a session
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Unauthenticated_ShouldReturnUnauthorized()
    {
        await SetupApiClientAsync();
        await LogoutAsync();

        if (ApiClient is not null)
        {
            Task<GetOrCreateMySessionResponse?> task = ApiClient.Account.My.Session.GetAsync();

            await task.ShouldThrowAsync<ApiException>();
        }
    }

    /// <summary>
    ///   Providing an invalid session ID should return 404
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task InvalidSessionId_ShouldReturn404()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        if (ApiClient is not null)
        {
            Task<GetOrCreateMySessionResponse?> task = ApiClient.Account.My.Session.GetAsync(req => req.QueryParameters.SessionId = Guid.NewGuid().ToString());

            await task.ShouldThrowAsync<ApiException>();
        }
    }
}
