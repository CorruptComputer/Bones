using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Backend.TestOperations.AccountManagement;
using Bones.Testing.Shared.Backend.TestOperations.Audit;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for getting user sessions with rate limiting
/// </summary>
public class GetBonesUserSessionDbTests : TestBase
{
    private readonly GetBonesUserSessionDb.Validator _validator = new();
    private readonly IPAddress _testIp = IPAddress.Parse("127.0.0.1");

    /// <summary>
    ///   Tests that a valid session request succeeds and is logged
    /// </summary>
    [Fact]
    public async Task ValidSession_ShouldSucceedAndLogAttempt()
    {
        // Create a test user and session
        (BonesUser user, BonesUserSession session) = await CreateTestUserAndSession();
        Guid sessionId = session.Id;

        // Get the session
        QueryResponse<BonesUserSession?> result = await Sender.Send(new GetBonesUserSessionDb.Query(sessionId, _testIp));
        result.Success.ShouldBeTrue();
        result.Result.ShouldNotBeNull();
        result.Result!.Id.ShouldBe(sessionId);

        // Check the audit log
        QueryResponse<List<SessionAttemptAudit>> auditLog = await Sender.Send(new GetSessionAttemptAuditsByIp.Query(_testIp));
        auditLog.Result.ShouldNotBeNull();
        auditLog.Result.Count.ShouldBe(1);
        auditLog.Result[0].Successful.ShouldBeTrue();
        auditLog.Result[0].IpAddress.ShouldBe(_testIp);
        auditLog.Result[0].SessionId.ShouldBe(sessionId);
    }

    /// <summary>
    ///   Tests that an invalid session request fails and is logged
    /// </summary>
    [Fact]
    public async Task InvalidSession_ShouldFailAndLogAttempt()
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
        // Create a test user and session
        (BonesUser user, BonesUserSession session) = await CreateTestUserAndSession();
        Guid sessionId = session.Id;

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
        QueryResponse<BonesUserSession?> result11 = await Sender.Send(new GetBonesUserSessionDb.Query(sessionId, _testIp));
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
        // Create a test user and session
        (BonesUser user, BonesUserSession session) = await CreateTestUserAndSession();
        Guid sessionId = session.Id;

        // Try to get a non-existent session
        QueryResponse<BonesUserSession?> attemptResult = await Sender.Send(new GetBonesUserSessionDb.Query(Guid.NewGuid(), _testIp));
        attemptResult.Success.ShouldBeFalse();
        attemptResult.Result.ShouldBeNull();
        attemptResult.FailureReasons.ShouldContainKey(BonesResponseBase.SERVER_ERROR_KEY);
        attemptResult.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].ShouldContain("Session not found.");

        // Try to get the valid session - should be rate limited
        QueryResponse<BonesUserSession?> result = await Sender.Send(new GetBonesUserSessionDb.Query(sessionId, _testIp));
        result.Success.ShouldBeTrue();
        result.Result.ShouldNotBeNull();
        result.Result!.Id.ShouldBe(sessionId);
    }

    private async Task<(BonesUser user, BonesUserSession session)> CreateTestUserAndSession()
    {
        // Create a test user
        CreateBonesUser.Query registerQuery = new CreateBonesUser.Query("test@example.com", "Password123!");
        QueryResponse<IdentityResult> registerResult = await Sender.Send(registerQuery);
        registerResult.Success.ShouldBeTrue();
        registerResult.Result.ShouldNotBeNull();
        registerResult.Result!.Succeeded.ShouldBeTrue();

        // Get the user
        QueryResponse<List<BonesUser>> users = await Sender.Send(new GetAllUsers.Query());
        users.Result.ShouldNotBeEmpty();
        BonesUser user = users.Result.First();

        // Create a session for the user
        CreateAndGetBonesUserSessionDb.Query createSessionQuery = new CreateAndGetBonesUserSessionDb.Query(user, _testIp, Convert.ToBase64String(new byte[32]));
        QueryResponse<BonesUserSession> session = await Sender.Send(createSessionQuery);
        session.Result.ShouldNotBeNull();

        return (user, session.Result!);
    }
} 