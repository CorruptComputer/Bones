using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.Accounts;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for setting password last set date time
/// </summary>
public class SetPasswordLastSetDateTimeDbTests : TestBase
{
    private readonly SetPasswordLastSetDateTimeDb.Validator validator = new();

    /// <summary>
    ///   Tests that the validator stops this
    /// </summary>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        SetPasswordLastSetDateTimeDb.Command command = new(Guid.Empty, default);

        TestValidationResult<SetPasswordLastSetDateTimeDb.Command> validationResult = await validator.TestValidateAsync(command);
        validationResult.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    /// <summary>
    ///   Tests that a valid request succeeds and updates the time
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldSucceedAndUpdateTime()
    {
        BonesUser user = await GetBackgroundServiceUserAsync();

        DateTimeOffset newLastSetDateTime = DateTimeOffset.UtcNow.AddHours(1);

        CommandResponse result = await Sender.Send(new SetPasswordLastSetDateTimeDb.Command(user.Id, newLastSetDateTime));
        result.Success.ShouldBeTrue();

        BonesUser userAfterUpdate = await GetBackgroundServiceUserAsync();
        userAfterUpdate.PasswordLastSetDateTime.ShouldBe(newLastSetDateTime);
    }

    /// <summary>
    ///   Tests that a invalid request fails
    /// </summary>
    [Fact]
    public async Task InvalidUserId_ShouldFail()
    {
        CommandResponse result = await Sender.Send(new SetPasswordLastSetDateTimeDb.Command(Guid.NewGuid(), DateTimeOffset.UtcNow));
        result.Success.ShouldBeFalse();
    }
}
