using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for updating user profile
/// </summary>
public class UpdateProfileDbTests : TestBase
{
    private readonly UpdateProfileDb.Validator _validator = new();

    /// <summary>
    ///   Tests that the validator stops invalid inputs
    /// </summary>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        UpdateProfileDb.Command command = new(Guid.Empty, string.Empty);

        TestValidationResult<UpdateProfileDb.Command> validationResult = await _validator.TestValidateAsync(command);
        validationResult.ShouldHaveValidationErrorFor(x => x.UserId);
        validationResult.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    /// <summary>
    ///   Tests that updating a user's display name succeeds
    /// </summary>
    /// <param name="displayName"></param>
    [Theory]
    [InlineData("New Display Name")]
    [InlineData("新しい表示名")]
    [InlineData("Nýtt birtingarheiti")]
    [InlineData("Emojis work ✅")]
    public async Task ValidDisplayName_ShouldUpdateDisplayName(string displayName)
    {
        BonesUser user = await GetBackgroundServiceUserAsync();

        CommandResponse response = await Sender.Send(new UpdateProfileDb.Command(user.Id, displayName));
        response.Success.ShouldBeTrue();

        // Verify the update persisted by querying the user by email
        BonesUser updatedUser = await GetBackgroundServiceUserAsync();
        updatedUser.DisplayName.ShouldBe(displayName);
    }

    /// <summary>
    ///   Tests that updating a non-existent user fails
    /// </summary>
    [Fact]
    public async Task NonExistentUserId_ShouldFail()
    {
        CommandResponse response = await Sender.Send(new UpdateProfileDb.Command(Guid.NewGuid(), "New Name"));

        response.Success.ShouldBeFalse();
    }
}
