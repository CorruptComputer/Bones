using System;
using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Exceptions;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for invalidating a user session
/// </summary>
public class InvalidateBonesUserSessionDbTests : TestBase
{
    private readonly InvalidateBonesUserSessionDb.Validator validator = new();
    private readonly IPAddress _testIp = IPAddress.Parse("127.0.0.1");

    /// <summary>
    ///   Tests that the validator stops this
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        InvalidateBonesUserSessionDb.Command query = new(Guid.Empty);

        TestValidationResult<InvalidateBonesUserSessionDb.Command> validationResult = await validator.TestValidateAsync(query);
        validationResult.ShouldHaveValidationErrorFor(x => x.SessionId);
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

        CommandResponse result = await Sender.Send(new InvalidateBonesUserSessionDb.Command(session.Id));
        result.Success.ShouldBeTrue();

        BonesUserSession? sessionAfterInvalidation = await Sender.Send(new GetBonesUserSessionDb.Query(session.Id, _testIp));
        sessionAfterInvalidation.ShouldNotBeNull();
        sessionAfterInvalidation.IsInvalidated.ShouldBeTrue();
    }

    /// <summary>
    ///   Tests that a valid request only invalidates the specified session
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldSucceedAndOnlyInvalidateSpecifiedSession()
    {
        BonesUser user = await GetBackgroundServiceUserAsync();

        // Create a couple sessions
        BonesUserSession? session = await Sender.Send(new CreateAndGetBonesUserSessionDb.Query(user, _testIp, string.Empty));
        BonesTestException.ThrowIfNull(session);

        BonesUserSession? session2 = await Sender.Send(new CreateAndGetBonesUserSessionDb.Query(user, _testIp, string.Empty));
        BonesTestException.ThrowIfNull(session2);

        // Only invalidate the second session
        CommandResponse result = await Sender.Send(new InvalidateBonesUserSessionDb.Command(session2.Id));
        result.Success.ShouldBeTrue();

        // Check the sessions
        BonesUserSession? sessionAfterInvalidation = await Sender.Send(new GetBonesUserSessionDb.Query(session.Id, _testIp));
        sessionAfterInvalidation.ShouldNotBeNull();
        sessionAfterInvalidation.IsInvalidated.ShouldBeFalse();

        BonesUserSession? session2AfterInvalidation = await Sender.Send(new GetBonesUserSessionDb.Query(session2.Id, _testIp));
        session2AfterInvalidation.ShouldNotBeNull();
        session2AfterInvalidation.IsInvalidated.ShouldBeTrue();
    }

    /// <summary>
    ///   Tests that an invalid session ID fails
    /// </summary>
    [Fact]
    public async Task InvalidSessionId_ShouldFail()
    {
        CommandResponse result = await Sender.Send(new InvalidateBonesUserSessionDb.Command(Guid.NewGuid()));
        result.Success.ShouldBeFalse();
    }
}
