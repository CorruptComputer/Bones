using System.Net;
using Bones.Database.Operations.Audits;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.Audit;

/// <summary>
///   Tests for getting login attempts for rate limiting
/// </summary>
public class GetLoginAttemptsForRateLimitingDbTests : TestBase
{
    private readonly IPAddress _testIp = IPAddress.Parse("127.0.0.1");

    /// <summary>
    ///   Tests that the validator stops invalid inputs
    /// </summary>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        GetLoginAttemptsForRateLimitingDb.Validator validator = new();
        GetLoginAttemptsForRateLimitingDb.Query query = new(null!, DateTimeOffset.UtcNow);

        TestValidationResult<GetLoginAttemptsForRateLimitingDb.Query> validationResult = await validator.TestValidateAsync(query);
        validationResult.ShouldHaveValidationErrorFor(x => x.RequestingIp);
    }

    /// <summary>
    ///   Tests that no login attempts returns zero
    /// </summary>
    [Fact]
    public async Task NoLoginAttempts_ShouldReturnZero()
    {
        DateTimeOffset cutoffTime = DateTimeOffset.UtcNow.AddMinutes(-10);
        GetLoginAttemptsForRateLimitingDb.Query query = new(_testIp, cutoffTime);

        QueryResponse<int> response = await Sender.Send(query);
        response.Success.ShouldBeTrue();
        response.Result.ShouldBe(0);
    }

    /// <summary>
    ///   Tests that failed login attempts within cutoff are counted
    /// </summary>
    [Fact]
    public async Task FailedLoginAttemptsWithinCutoff_ShouldBeCounted()
    {
        // Add some failed login attempts
        DateTimeOffset now = DateTimeOffset.UtcNow;
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", false, _testIp));
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", false, _testIp));
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", false, _testIp));

        GetLoginAttemptsForRateLimitingDb.Query query = new(_testIp, now.AddMinutes(-10));
        QueryResponse<int> response = await Sender.Send(query);

        response.Success.ShouldBeTrue();
        response.Result.ShouldBe(3);
    }

    /// <summary>
    ///   Tests that successful login attempts are not counted
    /// </summary>
    [Fact]
    public async Task SuccessfulLoginAttempts_ShouldNotBeCounted()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", true, _testIp));
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", true, _testIp));
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", false, _testIp));

        GetLoginAttemptsForRateLimitingDb.Query query = new(_testIp, now.AddMinutes(-10));
        QueryResponse<int> response = await Sender.Send(query);

        response.Success.ShouldBeTrue();
        response.Result.ShouldBe(1);
    }

    /// <summary>
    ///   Tests that attempts outside cutoff are not counted
    /// </summary>
    [Fact]
    public async Task AttemptsOutsideCutoff_ShouldNotBeCounted()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", false, _testIp));
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", false, _testIp));
        await Sender.Send(new AddLoginAuditDb.Command("test@example.com", false, _testIp));

        GetLoginAttemptsForRateLimitingDb.Query query = new(_testIp, now.AddMinutes(10));
        QueryResponse<int> response = await Sender.Send(query);

        response.Success.ShouldBeTrue();
        response.Result.ShouldBe(0);
    }
}