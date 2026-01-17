using System;

namespace Bones.Api.IntegrationTests.Account;

/// <summary>
///   Provides tests for the profile functionality of the Bones API.
/// </summary>
public class ProfileTests : TestBase
{
    /// <summary>
    ///   Default admin should be able to get their profile
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DefaultAdmin_ShouldBeAbleToGetProfile()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        GetMyProfileResponse? profile = null;
        if (ApiClient is not null)
        {
            Task<GetMyProfileResponse?> task = ApiClient.MyAccount.Profile.GetAsync();
            await task.ShouldNotThrowAsync();
            profile = await task;
        }

        profile.ShouldNotBeNull();
        profile.DisplayName.ShouldNotBeNull();
    }

    /// <summary>
    ///   Default admin should be able to update their profile
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DefaultAdmin_ShouldBeAbleToUpdateProfile()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        bool? successful = null;
        if (ApiClient is not null)
        {
            Task<bool?> task = ApiClient.MyAccount.Profile.PutAsync(new UpdateMyProfileRequest()
            {
                DisplayName = "New DisplayName"
            });
            await task.ShouldNotThrowAsync();
            successful = await task;
        }

        successful.ShouldNotBeNull();
        successful.ShouldBe(true);
    }
}
