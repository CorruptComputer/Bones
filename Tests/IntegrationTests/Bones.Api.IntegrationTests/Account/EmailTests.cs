namespace Bones.Api.IntegrationTests.Account;


/// <summary>
///   Provides tests for the email change functionality of the Bones API.
/// </summary>
public class EmailTests : TestBase
{
    /// <summary>
    ///   Default admin should be able to change their email
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "need to make an account specifically for this")]
    public async Task DefaultAdmin_ShouldBeAbleToChangeEmail()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        bool? successful = null;
        if (ApiClient is not null)
        {
            Task<bool?> task = ApiClient.MyAccount.Email.PutAsync(new ChangeMyEmailRequest()
            {
                NewEmail = "admin2@example.com"
            });
            await task.ShouldNotThrowAsync();
            successful = await task;
        }

        successful.ShouldNotBeNull();
        successful.ShouldBe(true);
    }
}
