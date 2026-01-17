using System;
using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for creating and returning a session
/// </summary>
public class CreateAndGetBonesUserSessionDbTests : TestBase
{
    private readonly CreateAndGetBonesUserSessionDb.Validator validator = new();
    private readonly IPAddress _testIp = IPAddress.Parse("127.0.0.1");

    /// <summary>
    ///   Tests that the validator stops this
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Validator_ShouldStopInvalidInputs()
    {
        CreateAndGetBonesUserSessionDb.Query query = new(Guid.Empty, IPAddress.None, string.Empty);

        TestValidationResult<CreateAndGetBonesUserSessionDb.Query> validationResult = await validator.TestValidateAsync(query);
        validationResult.ShouldHaveValidationErrorFor(x => x.RequestingUserId);
        validationResult.ShouldHaveValidationErrorFor(x => x.RequestingIp);
    }

    /// <summary>
    ///   Tests that a valid request succeeds
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldSucceedAndReturnSession()
    {
        BonesUser user = await GetBackgroundServiceUserAsync();

        // Get the session
        QueryResponse<BonesUserSession> result = await Sender.Send(new CreateAndGetBonesUserSessionDb.Query(user.Id, _testIp, string.Empty));
        result.Success.ShouldBeTrue();
        result.Result.ShouldNotBeNull();
        result.Result.IpAddress.ShouldBe(_testIp);
    }
}
