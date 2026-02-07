using System.Net;
using Bones.Database.Operations.Audits;
using Bones.Logic.Features.Accounts;
using Bones.Shared.Backend.Models;
using Bones.Testing.UnitTests.Shared;
using FluentValidation.TestHelper;

namespace Bones.Logic.UnitTests.Features.Accounts;

/// <summary>
///   Tests for checking login rate limit and password expiration
/// </summary>
public class CheckLoginRateLimitAndPasswordExpiredTests : TestBase
{
    private readonly CheckLoginRateLimit.Validator _validator = new();
    private readonly IPAddress _testIp = IPAddress.Parse("127.0.0.1");

    /// <summary>
    ///   Tests that the validator stops invalid inputs
    /// </summary>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        CheckLoginRateLimit.Query nullIpQuery = new(null!);
        TestValidationResult<CheckLoginRateLimit.Query> nullIpResult = await _validator.TestValidateAsync(nullIpQuery);
        nullIpResult.ShouldHaveValidationErrorFor(x => x.RequestingIp);
    }

    /// <summary>
    ///   Tests that a new IP with no failed attempts is allowed
    /// </summary>
    [Fact]
    public async Task NewIp_ShouldBeAllowed()
    {
        QueryResponse<bool> response = await Sender.Send(new CheckLoginRateLimit.Query(_testIp));

        response.Success.ShouldBeTrue();
        response.Result.ShouldBeTrue();
    }

    /// <summary>
    ///   Tests that an IP with too many failed attempts is blocked
    /// </summary>
    [Fact]
    public async Task TooManyFailedAttempts_ShouldBeBlocked()
    {
        const string email = "test@example.com";

        // Add failed login attempts
        for (int i = 0; i < 10; i++)
        {
            await Sender.Send(new AddLoginAuditDb.Command(email, false, _testIp));
        }

        QueryResponse<bool> response = await Sender.Send(new CheckLoginRateLimit.Query(_testIp));

        response.Success.ShouldBeTrue();
        response.Result.ShouldBeFalse();
    }
}