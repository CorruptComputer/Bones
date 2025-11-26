using System;
using System.Runtime.InteropServices;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.AccountManagement;

/// <summary>
///   Tests for getting a user by email
/// </summary>
public class GetUserByEmailDbTests : TestBase
{
    private readonly GetUserByEmailDb.Validator validator = new();

    /// <summary>
    ///   Tests that the validator stops this
    /// </summary>
    /// <returns></returns>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not an email address")]
    public async Task Validator_ShouldStopInvalidInputs(string? email)
    {
        GetUserByEmailDb.Query query = new(email!);

        TestValidationResult<GetUserByEmailDb.Query> validationResult = await validator.TestValidateAsync(query);
        validationResult.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///   Tests that a valid email should return the associated user
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ValidEmail_ShouldBeReturned()
    {
        BonesUser user = await GetBackgroundServiceUserAsync();

        QueryResponse<BonesUser?> result = await Sender.Send(new GetUserByEmailDb.Query(user.Email!));
        result.Success.ShouldBeTrue();
        result.Result.ShouldNotBeNull();
        result.Result.Id.ShouldBe(user.Id);
    }

    /// <summary>
    ///   Tests that a valid email should return the associated user
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task InvalidEmail_ShouldFail()
    {
        QueryResponse<BonesUser?> result = await Sender.Send(new GetUserByEmailDb.Query("not-a-valid-email@example.com"));
        result.Success.ShouldBeFalse();
        result.Result.ShouldBeNull();
    }
}
