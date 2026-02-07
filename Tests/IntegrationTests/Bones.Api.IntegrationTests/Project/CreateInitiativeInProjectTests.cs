namespace Bones.Api.IntegrationTests.Project;

/// <summary>
///   Provides tests for the initiative creation functionality of the Bones API.
/// </summary>
public class CreateInitiativeInProjectTests : TestBase
{
    /// <summary>
    ///   Default admin should be able to create an initiative in a project
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DefaultAdmin_ShouldBeAbleToCreateAnInitiativeInAProject()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        if (ApiClient is null)
        {
            throw new InvalidOperationException("ApiClient is not initialized");
        }

        Guid? projectId = await ApiClient.Project.Create.PostAsync(new()
        {
            Name = "test project",
            OrganizationId = null
        });

        Guid? initiativeId = await ApiClient.Project[projectId.Value].Initiative.Create.PostAsync(new CreateInitiativeRequest()
        {
            Name = "Test Initiative"
        });

        initiativeId.ShouldNotBeNull();
        initiativeId.ShouldNotBe(Guid.Empty);
    }
}
