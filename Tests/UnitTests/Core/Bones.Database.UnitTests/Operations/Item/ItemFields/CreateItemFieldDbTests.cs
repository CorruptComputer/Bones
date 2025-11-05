using Bones.Database.Operations.Item;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.Item.ItemFields;

/// <summary>
///   Tests for creating item fields
/// </summary>
public class CreateItemFieldDbTests : TestBase
{
    private readonly CreateItemFieldDb.Validator _validator = new();

    /// <summary>
    ///   Checks that the handler stops this.
    /// </summary>
    [Fact]
    public async Task InvalidProjectId_ShouldFail()
    {
        CreateItemFieldDb.Command command = new(Guid.Empty);
        TestValidationResult<CreateItemFieldDb.Command> validationResult = await _validator.TestValidateAsync(command);
        validationResult.ShouldHaveValidationErrors();

        CommandResponse result = await Sender.Send(command);
        result.Success.ShouldBeFalse();
    }

    /// <summary>
    ///   Checks that the handler stops this.
    /// </summary>
    [Fact]
    public async Task NonExistentProject_ShouldFail()
    {
        CreateItemFieldDb.Command command = new(Guid.NewGuid());
        TestValidationResult<CreateItemFieldDb.Command> validationResult = await _validator.TestValidateAsync(command);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse result = await Sender.Send(command);
        result.Success.ShouldBeFalse();
        result.FailureReasons.ShouldContainKey(BonesResponseBase.SERVER_ERROR_KEY);
        result.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].ShouldContain("Project not found");
    }

    /// <summary>
    ///   Checks that the handler stops this.
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldPass()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        CreateItemFieldDb.Command command = new(projectId);
        TestValidationResult<CreateItemFieldDb.Command> validationResult = await _validator.TestValidateAsync(command);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse result = await Sender.Send(command);
        result.Success.ShouldBeTrue();
    }

    /// <summary>
    ///   Checks that the handler handles concurrent access correctly.
    /// </summary>
    [Fact]
    public async Task ConcurrentAccess_ShouldHandleCorrectly()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        CreateItemFieldDb.Command command = new(projectId);
        TestValidationResult<CreateItemFieldDb.Command> validationResult = await _validator.TestValidateAsync(command);
        validationResult.ShouldNotHaveAnyValidationErrors();

        // Send multiple requests concurrently
        Task<CommandResponse>[] tasks = Enumerable.Range(0, 5)
            .Select(_ => Sender.Send(command))
            .ToArray();

        CommandResponse[] results = await Task.WhenAll(tasks);

        // All should succeed since we're creating new fields
        results.All(r => r.Success).ShouldBeTrue();
    }
}
