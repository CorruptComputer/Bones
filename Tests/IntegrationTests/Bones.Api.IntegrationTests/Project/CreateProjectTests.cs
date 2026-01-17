namespace Bones.Api.IntegrationTests.Project;

/// <summary>
///   Provides tests for the project creation functionality of the Bones API.
/// </summary>
public class CreateProjectTests : TestBase
{
    /// <summary>
    ///   Default admin should be able to create a project
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DefaultAdmin_ShouldBeAbleToCreateAProject()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.DefaultAdmin);

        if (ApiClient is null)
        {
            throw new InvalidOperationException("ApiClient is not initialized");
        }

        Guid? projectId = await ApiClient.Project.Create.PostAsync(new()
        {
            Name = "test",
            OrganizationId = null
        });

        projectId.ShouldNotBeNull();
        projectId.ShouldNotBe(Guid.Empty);
    }

    /// <summary>
    ///   Default admin should be able to create a project
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
            Name = "development preset test",
            Preset = preset,
            OrganizationId = null
        });

        projectId.ShouldNotBeNull();
        projectId.ShouldNotBe(Guid.Empty);
    }
}