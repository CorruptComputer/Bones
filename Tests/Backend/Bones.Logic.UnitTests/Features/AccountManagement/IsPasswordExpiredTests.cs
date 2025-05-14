using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Accounts;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Backend;
using Bones.Testing.Shared.Backend.TestOperations.AccountManagement;
using FluentValidation.TestHelper;

namespace Bones.Logic.UnitTests.Features.AccountManagement;

/// <summary>
///   Tests for the IsPasswordExpired feature
/// </summary>
public class IsPasswordExpiredTests : TestBase
{
    private readonly IsPasswordExpired.Validator _validator = new();

    /// <summary>
    ///   Tests that the validator stops invalid inputs
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task Validator_ShouldStopInvalidInputs(string? invalidEmail)
    {
        IsPasswordExpired.Query query = new(invalidEmail!);
        TestValidationResult<IsPasswordExpired.Query> result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///   Tests that a user with expired password is blocked
    /// </summary>
    [Fact]
    public async Task ExpiredPassword_ShouldBeBlocked()
    {
        // Create a user with expired password
        const string email = "expired@example.com";
        const string password = "Password123!";

        await Sender.Send(new CreateBonesUser.Query(email, password));
        BonesUser? user = await GetUserByEmailAsync(email);
        user.ShouldNotBeNull();
        user.PasswordExpired = true;

        QueryResponse<bool> response = await Sender.Send(new IsPasswordExpired.Query(email));

        response.Success.ShouldBeTrue();
        response.Result.ShouldBeFalse();
    }

    /// <summary>
    ///   Tests that a user with valid password is allowed
    /// </summary>
    [Fact]
    public async Task ValidPassword_ShouldBeAllowed()
    {
        // Create a user with valid password
        const string email = "valid@example.com";
        const string password = "Password123!";

        await Sender.Send(new CreateBonesUser.Query(email, password));

        QueryResponse<bool> response = await Sender.Send(new IsPasswordExpired.Query(email));

        response.Success.ShouldBeTrue();
        response.Result.ShouldBeFalse();
    }

    /// <summary>
    ///   Tests that a non-existent user is allowed (password expiry check is skipped)
    /// </summary>
    [Fact]
    public async Task NonExistentUser_ShouldBeAllowed()
    {
        const string email = "nonexistent@example.com";
        QueryResponse<bool> response = await Sender.Send(new IsPasswordExpired.Query(email));

        response.Success.ShouldBeTrue();
        response.Result.ShouldBeFalse();
    }

    private async Task<BonesUser?> GetUserByEmailAsync(string email)
    {
        QueryResponse<List<BonesUser>> users = await Sender.Send(new GetAllUsers.Query());
        return users.Result?.FirstOrDefault();
    }
}
