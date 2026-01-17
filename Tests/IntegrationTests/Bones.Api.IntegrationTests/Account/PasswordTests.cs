using System;

namespace Bones.Api.IntegrationTests.Account;

/// <summary>
///   Provides tests for the password change functionality of the Bones API.
/// </summary>
public class PasswordTests : TestBase
{
    /// <summary>
    ///   Default admin should be able to change their password
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "need to make an account specifically for this")]
    public async Task DefaultAdmin_ShouldBeAbleToChangePassword()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        bool? successful = null;
        if (ApiClient is not null)
        {
            Task<bool?> task = ApiClient.MyAccount.Password.PutAsync(new ChangeMyPasswordRequest()
            {
                CurrentPassword = TestCredentials.Credentials[TestCredentials.User.DefaultAdmin].password,
                InvalidateOtherSessions = false,
                NewPassword = "newPassword!123"
            });
            await task.ShouldNotThrowAsync();
            successful = await task;
        }

        successful.ShouldNotBeNull();
        successful.ShouldBe(true);
    }
}