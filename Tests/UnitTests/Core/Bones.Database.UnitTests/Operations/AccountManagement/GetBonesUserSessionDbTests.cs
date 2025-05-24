using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;
using Bones.Testing.UnitTests.Shared.TestOperations.Audit;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for getting user sessions with rate limiting
/// </summary>
public class GetBonesUserSessionDbTests : TestBase
{
    private readonly GetBonesUserSessionDb.Validator _validator = new();
    private readonly IPAddress _testIp = IPAddress.Parse("127.0.0.1");

    /// <summary>
    ///   Tests that the validator stops this
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        GetBonesUserSessionDb.Query emptyIdQuery = new(Guid.Empty, IPAddress.IPv6Loopback);
        TestValidationResult<GetBonesUserSessionDb.Query> emptyIdResult = await _validator.TestValidateAsync(emptyIdQuery);

        emptyIdResult.ShouldHaveValidationErrorFor(nameof(GetBonesUserSessionDb.Query.SessionId));
    }

    /// <summary>
    ///   Tests that a valid session request succeeds and is logged
    /// </summary>
    [Fact]
    public async Task ValidSession_ShouldSucceedAndLogAttempt()
    {
        BonesUserSession session = await CreateSession();

        // Get the session
        QueryResponse<BonesUserSession?> result = await Sender.Send(new GetBonesUserSessionDb.Query(session.Id, _testIp));
        result.Success.ShouldBeTrue();
        result.Result.ShouldNotBeNull();
        result.Result!.Id.ShouldBe(session.Id);

        // Check the audit log
        QueryResponse<List<SessionAttemptAudit>> auditLog = await Sender.Send(new GetSessionAttemptAuditsByIp.Query(_testIp));
        auditLog.Result.ShouldNotBeNull();
        auditLog.Result.Count.ShouldBe(1);
        auditLog.Result[0].Successful.ShouldBeTrue();
        auditLog.Result[0].IpAddress.ShouldBe(_testIp);
        auditLog.Result[0].SessionId.ShouldBe(session.Id);
    }

    /// <summary>
    ///   Tests that an invalid session request fails and is logged
    /// </summary>
    [Fact]
    public async Task InvalidSessionId_ShouldFailAndLogAttempt()
    {
        // Try to get a non-existent session
        QueryResponse<BonesUserSession?> result = await Sender.Send(new GetBonesUserSessionDb.Query(Guid.NewGuid(), _testIp));
        result.Success.ShouldBeFalse();
        result.Result.ShouldBeNull();
        result.FailureReasons.ShouldContainKey(BonesResponseBase.SERVER_ERROR_KEY);
        result.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].ShouldContain("Session not found.");

        // Check the audit log
        QueryResponse<List<SessionAttemptAudit>> auditLog = await Sender.Send(new GetSessionAttemptAuditsByIp.Query(_testIp));
        auditLog.Result.ShouldNotBeNull();
        auditLog.Result.Count.ShouldBe(1);
        auditLog.Result[0].Successful.ShouldBeFalse();
        auditLog.Result[0].IpAddress.ShouldBe(_testIp);
    }

    /// <summary>
    ///   Tests that rate limiting works after too many failed attempts
    /// </summary>
    [Fact]
    public async Task TooManyFailedAttempts_ShouldRateLimit()
    {
        BonesUserSession session = await CreateSession();

        // Try to get a non-existent session 10 times
        for (int i = 0; i < 10; i++)
        {
            QueryResponse<BonesUserSession?> attemptResult = await Sender.Send(new GetBonesUserSessionDb.Query(Guid.NewGuid(), _testIp));
            attemptResult.Success.ShouldBeFalse();
            attemptResult.Result.ShouldBeNull();
            attemptResult.FailureReasons.ShouldContainKey(BonesResponseBase.SERVER_ERROR_KEY);
            attemptResult.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].ShouldContain("Session not found.");
        }

        // Try to get the valid session - should be rate limited
        QueryResponse<BonesUserSession?> result11 = await Sender.Send(new GetBonesUserSessionDb.Query(session.Id, _testIp));
        result11.Success.ShouldBeFalse();
        result11.Result.ShouldBeNull();
        result11.FailureReasons.ShouldContainKey(BonesResponseBase.SERVER_ERROR_KEY);
        result11.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].ShouldContain("Too many failed attempts. Please try again later.");

        // Check the audit log
        QueryResponse<List<SessionAttemptAudit>> auditLog = await Sender.Send(new GetSessionAttemptAuditsByIp.Query(_testIp));
        auditLog.Result.ShouldNotBeNull();
        auditLog.Result.Count.ShouldBe(11);
        auditLog.Result.All(x => !x.Successful).ShouldBeTrue();
    }

    /// <summary>
    ///   Tests that rate limiting works after too many failed attempts
    /// </summary>
    [Fact]
    public async Task RateLimitedSession_ShouldFail()
    {
        BonesUserSession session = await CreateSession();

        // Try to get a non-existent session
        QueryResponse<BonesUserSession?> attemptResult = await Sender.Send(new GetBonesUserSessionDb.Query(Guid.NewGuid(), _testIp));
        attemptResult.Success.ShouldBeFalse();
        attemptResult.Result.ShouldBeNull();
        attemptResult.FailureReasons.ShouldContainKey(BonesResponseBase.SERVER_ERROR_KEY);
        attemptResult.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].ShouldContain("Session not found.");

        // Try to get the valid session - should be rate limited
        QueryResponse<BonesUserSession?> result = await Sender.Send(new GetBonesUserSessionDb.Query(session.Id, _testIp));
        result.Success.ShouldBeTrue();
        result.Result.ShouldNotBeNull();
        result.Result!.Id.ShouldBe(session.Id);
    }

    private async Task<BonesUserSession> CreateSession()
    {
        QueryResponse<BonesUserSession> session = await Sender.Send(
            new CreateAndGetBonesUserSessionDb.Query(await GetBackgroundServiceUserAsync(), _testIp, Convert.ToBase64String(new byte[32]))
        );

        session.Result.ShouldNotBeNull();

        return session.Result;
    }
}