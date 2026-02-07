namespace Bones.Api.IntegrationTests.Account;


/// <summary>
///   Provides tests for the email change functionality of the Bones API.
/// </summary>
public class EmailTests : TestBase
{
    /// <summary>
    ///   Change email user should be able to change their email
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ChangeEmailUser_ShouldBeAbleToChangeEmail()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.ChangeEmail);

        bool? successful = null;
        if (ApiClient is not null)
        {
            Task<bool?> task = ApiClient.MyAccount.Email.PutAsync(new ChangeMyEmailRequest()
            {
                NewEmail = "change-email2@example.com"
            });
            await task.ShouldNotThrowAsync();
            successful = await task;
        }

        successful.ShouldNotBeNull();
        successful.ShouldBe(true);
    }
}
