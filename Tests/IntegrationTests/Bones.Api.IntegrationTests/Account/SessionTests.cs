using System;

namespace Bones.Api.IntegrationTests.Account;
/// <summary>
///   Provides tests for the session functionality of the Bones API.
/// </summary>
public class SessionTests : TestBase
{
    /// <summary>
    /// 
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
            Task<GetOrCreateMySessionResponse> task = ApiClient.GetOrCreateMySessionAsync();
            await task.ShouldNotThrowAsync();
            session = await task;
        }

        session.ShouldNotBeNull();
        session.SessionId.ShouldNotBe(Guid.Empty);
        session.Base64LocalStorageKey.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Unauthenticated_ShouldReturnUnauthorized()
    {
        await SetupApiClientAsync();
        await LogoutAsync();

        if (ApiClient is not null)
        {
            Task<GetOrCreateMySessionResponse> task = ApiClient.GetOrCreateMySessionAsync();

            await task.ShouldThrowAsync<ApiException>();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task InvalidSessionId_ShouldReturn404()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        if (ApiClient is not null)
        {
            Task<GetOrCreateMySessionResponse> task = ApiClient.GetOrCreateMySessionAsync(Guid.NewGuid().ToString());

            await task.ShouldThrowAsync<ApiException<ErrorResponse>>();
        }
    }
}
