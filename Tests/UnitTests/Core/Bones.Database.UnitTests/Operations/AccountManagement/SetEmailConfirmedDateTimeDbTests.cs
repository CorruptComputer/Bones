using System;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for setting email confirmed date time
/// </summary>
public class SetEmailConfirmedDateTimeDbTests : TestBase
{
    private readonly SetEmailConfirmedDateTimeDb.Validator validator = new();

    /// <summary>
    ///   Tests that the validator stops this
    /// </summary>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        SetEmailConfirmedDateTimeDb.Command command = new(Guid.Empty, default);

        TestValidationResult<SetEmailConfirmedDateTimeDb.Command> validationResult = await validator.TestValidateAsync(command);
        validationResult.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    /// <summary>
    ///   Tests that a valid request succeeds and updates the time
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldSucceedAndUpdateTime()
    {
        BonesUser user = await GetBackgroundServiceUserAsync();

        DateTimeOffset newConfirmedDateTime = DateTimeOffset.UtcNow.AddHours(1);

        CommandResponse result = await Sender.Send(new SetEmailConfirmedDateTimeDb.Command(user.Id, newConfirmedDateTime));
        result.Success.ShouldBeTrue();

        BonesUser userAfterUpdate = await GetBackgroundServiceUserAsync();
        userAfterUpdate.EmailConfirmed.ShouldBeTrue();
        userAfterUpdate.EmailConfirmedDateTime.ShouldBe(newConfirmedDateTime);
    }

    /// <summary>
    ///   Tests that a invalid request fails
    /// </summary>
    [Fact]
    public async Task InvalidUserId_ShouldFail()
    {
        CommandResponse result = await Sender.Send(new SetEmailConfirmedDateTimeDb.Command(Guid.NewGuid(), DateTimeOffset.UtcNow));
        result.Success.ShouldBeFalse();
    }
}
