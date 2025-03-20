using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Backend;
using Bones.Testing.Shared.Backend.TestOperations.AccountManagement;
using Bones.Testing.Shared.Backend.TestOperations.Audit;
using FluentValidation.TestHelper;
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
        (BonesUser, BonesUserSession) user = await CreateTestUserWithSession();
        Guid sessionId = user.Item2.Id;

        // Attempt to get the session
        GetBonesUserSessionDb.Query query = new GetBonesUserSessionDb.Query(sessionId, _testIp);
        
        // Validate request
        TestValidationResult<GetBonesUserSessionDb.Query> validationResult = await _validator.TestValidateAsync(query);
        validationResult.ShouldNotHaveAnyValidationErrors();

        // Get session
        QueryResponse<BonesUserSession?> result = await Sender.Send(query);
        result.Success.Should().BeTrue();
        result.Result.Should().NotBeNull();
        result.Result!.Id.Should().Be(sessionId);

        // Verify audit log
        QueryResponse<List<SessionAttemptAudit>> auditLog = await Sender.Send(new GetSessionAttemptAudits.Query(_testIp));
        auditLog.Result.Should().HaveCount(1);
        auditLog.Result[0].Successful.Should().BeTrue();
        auditLog.Result[0].IpAddress.Should().Be(_testIp);
        auditLog.Result[0].SessionId.Should().Be(sessionId);
    }

    /// <summary>
    ///   Tests that an invalid session request fails and is logged
    /// </summary>
    [Fact]
    public async Task InvalidSession_ShouldFailAndLogAttempt()
    {
        // Attempt to get a non-existent session
        GetBonesUserSessionDb.Query query = new GetBonesUserSessionDb.Query(Guid.NewGuid(), _testIp);
        
        // Validate request
        TestValidationResult<GetBonesUserSessionDb.Query> validationResult = await _validator.TestValidateAsync(query);
        validationResult.ShouldNotHaveAnyValidationErrors();

        // Get session
        QueryResponse<BonesUserSession?> result = await Sender.Send(query);
        result.Success.Should().BeFalse();
        result.Result.Should().BeNull();
        result.FailureReasons.Should().ContainKey(BonesResponseBase.SERVER_ERROR_KEY);
        result.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].Should().Contain("Session not found.");

        // Verify audit log
        QueryResponse<List<SessionAttemptAudit>> auditLog = await Sender.Send(new GetSessionAttemptAudits.Query(_testIp));
        auditLog.Result.Should().HaveCount(1);
        auditLog.Result[0].Successful.Should().BeFalse();
        auditLog.Result[0].IpAddress.Should().Be(_testIp);
    }

    /// <summary>
    ///   Tests that rate limiting blocks requests after too many failed attempts
    /// </summary>
    [Fact]
    public async Task TooManyFailedAttempts_ShouldBeBlocked()
    {
        Guid nonExistentSessionId = Guid.NewGuid();
        GetBonesUserSessionDb.Query query = new GetBonesUserSessionDb.Query(nonExistentSessionId, _testIp);

        // Make 10 failed attempts
        for (int i = 0; i < 10; i++)
        {
            QueryResponse<BonesUserSession?> attemptResult = await Sender.Send(query);
            attemptResult.Success.Should().BeFalse();
            attemptResult.Result.Should().BeNull();
            attemptResult.FailureReasons.Should().ContainKey(BonesResponseBase.SERVER_ERROR_KEY);
            attemptResult.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].Should().Contain("Session not found.");
        }

        // Verify the 11th attempt is blocked
        QueryResponse<BonesUserSession?> result11 = await Sender.Send(query);
        result11.Success.Should().BeFalse();
        result11.Result.Should().BeNull();
        result11.FailureReasons.Should().ContainKey(BonesResponseBase.SERVER_ERROR_KEY);
        result11.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].Should().Contain("Too many failed attempts. Please try again later.");

        // Verify audit log shows 11 failed attempts
        QueryResponse<List<SessionAttemptAudit>> auditLog = await Sender.Send(new GetSessionAttemptAudits.Query(_testIp));
        auditLog.Result.Should().HaveCount(11);
        auditLog.Result.All(x => !x.Successful).Should().BeTrue();
    }

    /// <summary>
    ///   Tests that rate limiting resets after the timeout period
    /// </summary>
    [Fact]
    public async Task RateLimitingResetsAfterTimeout()
    {
        // Create a test user and session
        (BonesUser, BonesUserSession) user = await CreateTestUserWithSession();
        Guid sessionId = user.Item2.Id;

        Guid nonExistentSessionId = Guid.NewGuid();
        GetBonesUserSessionDb.Query failQuery = new GetBonesUserSessionDb.Query(nonExistentSessionId, _testIp);

        // Make 10 failed attempts
        for (int i = 0; i < 10; i++)
        {
            QueryResponse<BonesUserSession?> attemptResult = await Sender.Send(failQuery);
            attemptResult.Success.Should().BeFalse();
            attemptResult.Result.Should().BeNull();
            attemptResult.FailureReasons.Should().ContainKey(BonesResponseBase.SERVER_ERROR_KEY);
            attemptResult.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].Should().Contain("Session not found.");
        }

        // Modify the timestamp of the audit entries to be older than 10 minutes
        await Sender.Send(new UpdateSessionAttemptAuditTimes.Command(DateTimeOffset.UtcNow.AddMinutes(-11)));

        // Try a valid session request - should succeed now
        GetBonesUserSessionDb.Query validQuery = new GetBonesUserSessionDb.Query(sessionId, _testIp);
        QueryResponse<BonesUserSession?> result = await Sender.Send(validQuery);
        result.Success.Should().BeTrue();
        result.Result.Should().NotBeNull();
        result.Result!.Id.Should().Be(sessionId);
    }

    private async Task<(BonesUser, BonesUserSession)> CreateTestUserWithSession()
    {
        // Create a test user
        CreateBonesUser.Query registerQuery = new CreateBonesUser.Query("test@example.com", "Password123!");
        QueryResponse<IdentityResult> registerResult = await Sender.Send(registerQuery);
        registerResult.Success.Should().BeTrue();
        registerResult.Result.Should().NotBeNull();
        registerResult.Result!.Succeeded.Should().BeTrue();

        // Get the user
        QueryResponse<List<BonesUser>> users = await Sender.Send(new GetAllUsers.Query());
        users.Result.Should().NotBeEmpty();
        BonesUser user = users.Result.First();

        // Create a session for the user
        CreateAndGetBonesUserSessionDb.Query createSessionQuery = new CreateAndGetBonesUserSessionDb.Query(user, _testIp, Convert.ToBase64String(new byte[32]));
        QueryResponse<BonesUserSession> session = await Sender.Send(createSessionQuery);
        session.Result.Should().NotBeNull();

        return (user, session.Result!);
    }
} 