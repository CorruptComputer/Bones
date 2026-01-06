
using ReQuesty.Runtime;

namespace Bones.Api.IntegrationTests.Project;

/// <summary>
///   Provides tests for the session functionality of the Bones API.
/// </summary>
public class CreateProjectTests : TestBase
{
    /// <summary>
    ///   Default admin should be able to start a session
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DefaultAdmin_ShouldBeAbleToCreateAProject_WithDevelopmentPreset()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        if (ApiClient is null)
        {
            throw new InvalidOperationException("ApiClient is not initialized");
        }

        ProjectPreset? preset = ProjectPreset.Development;

        Guid? projectId = await ApiClient.Project.Create.PostAsync(new()
        {
            Name = "test",
            Preset = preset,
            OrganizationId = null
        });

        projectId.ShouldNotBeNull();
        projectId.ShouldNotBe(Guid.Empty);
    }
}