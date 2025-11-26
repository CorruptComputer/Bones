using System;
using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Exceptions;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for invalidating all sessions for a user
/// </summary>
public class InvalidateAllSessionsForUserDbTests : TestBase
{
    private readonly InvalidateAllSessionsForUserDb.Validator validator = new();
    private readonly IPAddress _testIp = IPAddress.Parse("127.0.0.1");

    /// <summary>
    ///   Tests that the validator stops this
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        InvalidateAllSessionsForUserDb.Command query = new(Guid.Empty, null);

        TestValidationResult<InvalidateAllSessionsForUserDb.Command> validationResult = await validator.TestValidateAsync(query);
        validationResult.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    /// <summary>
    ///   Tests that a valid request succeeds
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldSucceedAndInvalidateSession()
    {
        BonesUser user = await GetBackgroundServiceUserAsync();

        // Create a session
        BonesUserSession? session = await Sender.Send(new CreateAndGetBonesUserSessionDb.Query(user, _testIp, string.Empty));
        BonesTestException.ThrowIfNull(session);

        CommandResponse result = await Sender.Send(new InvalidateAllSessionsForUserDb.Command(user.Id, null));
        result.Success.ShouldBeTrue();

        BonesUserSession? sessionAfterInvalidation = await Sender.Send(new GetBonesUserSessionDb.Query(session.Id, _testIp));
        sessionAfterInvalidation.ShouldNotBeNull();
        sessionAfterInvalidation.IsInvalidated.ShouldBeTrue();
    }

        /// <summary>
    ///   Tests that a valid request with exclusions succeeds
    /// </summary>
    [Fact]
    public async Task ValidRequestWithExclusions_ShouldSucceedAndInvalidateNonexcludedSessions()
    {
        BonesUser user = await GetBackgroundServiceUserAsync();

        // Create a couple sessions
        BonesUserSession? session = await Sender.Send(new CreateAndGetBonesUserSessionDb.Query(user, _testIp, string.Empty));
        BonesTestException.ThrowIfNull(session);

        BonesUserSession? session2 = await Sender.Send(new CreateAndGetBonesUserSessionDb.Query(user, _testIp, string.Empty));
        BonesTestException.ThrowIfNull(session2);

        // Invalidate all but the first session
        CommandResponse result = await Sender.Send(new InvalidateAllSessionsForUserDb.Command(user.Id, [session.Id]));
        result.Success.ShouldBeTrue();

        // Check the sessions
        BonesUserSession? sessionAfterInvalidation = await Sender.Send(new GetBonesUserSessionDb.Query(session.Id, _testIp));
        sessionAfterInvalidation.ShouldNotBeNull();
        sessionAfterInvalidation.IsInvalidated.ShouldBeFalse();

        BonesUserSession? session2AfterInvalidation = await Sender.Send(new GetBonesUserSessionDb.Query(session2.Id, _testIp));
        session2AfterInvalidation.ShouldNotBeNull();
        session2AfterInvalidation.IsInvalidated.ShouldBeTrue();
    }
}
