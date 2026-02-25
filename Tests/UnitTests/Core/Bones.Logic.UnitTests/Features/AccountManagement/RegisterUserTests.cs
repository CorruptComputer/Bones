using Bones.Logic.Features.Accounts;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.System.Queues;
using Bones.Shared.Backend.Models;
using Bones.Testing.UnitTests.Shared;
using Bones.Testing.UnitTests.Shared.TestOperations.AccountManagement;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.UnitTests.Features.AccountManagement;

/// <summary>
///   Tests for user registration
/// </summary>
public class RegisterUserTests : TestBase
{
    private readonly RegisterUser.Validator _validator = new();

    /// <summary>
    ///   Checks that the validator and handler both validate the email address.
    /// </summary>
    /// <param name="email">The email address to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(" @ ")]
    [InlineData("InvalidEmail@example,com")]
    [InlineData("InvalidEmail@example.")]
    [InlineData("InvalidEmail@example")]
    [InlineData("InvalidEmail@")]
    [InlineData("InvalidEmail")]
    public async Task InvalidEmail_ShouldFail(string? email)
    {
        RegisterUser.Query request = new(email!, "abcdEFGH1!");

        TestValidationResult<RegisterUser.Query> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldHaveValidationErrorFor(x => x.Email);

        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.ShouldBeFalse();
        result.Result?.Succeeded.ShouldBeFalse();
    }

    /// <summary>
    ///   Checks that the validator and handler both validate the password.
    /// </summary>
    /// <param name="password">The password to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("aA1!")] // Not long enough
    [InlineData("abcdefgh1!")] // No caps
    [InlineData("ABCDEFGH1!")] // No lowercase
    [InlineData("abcdEFGH!")] // No number
    [InlineData("abcdEFGH1")] // No special char
    public async Task InvalidPassword_ShouldFail(string? password)
    {
        RegisterUser.Query request = new("test@example.com", password!);

        TestValidationResult<RegisterUser.Query> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldHaveValidationErrorFor(x => x.Password);

        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.ShouldBeFalse();
        result.Result?.Succeeded.ShouldBeFalse();
    }

    /// <summary>
    ///   Checks that the validator and handler both pass this.
    /// </summary>
    [Fact]
    public async Task ValidEmailAndPassword_ShouldSucceed()
    {
        RegisterUser.Query request = new("ValidEmailAndPassword@example.com", "abcdEFGH1!");

        TestValidationResult<RegisterUser.Query> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldNotHaveAnyValidationErrors();

        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.ShouldBeTrue();
        result.Result?.Succeeded.ShouldBeTrue();
    }

    /// <summary>
    ///   Checks that the handler actually queues a confirmation email for the new account
    /// </summary>
    [Fact]
    public async Task RegisteringUser_ShouldQueueConfirmationEmail()
    {
        RegisterUser.Query request = new("RegisteringUserQueueConfirmationEmail@example.com", "abcdEFGH1!");
        TestValidationResult<RegisterUser.Query> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldNotHaveAnyValidationErrors();

        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.ShouldBeTrue();
        result.Result?.Succeeded.ShouldBeTrue();

        List<BonesUser>? allUsers = await Sender.Send(new GetAllUsers.Query());
        BonesUser? createdUser = allUsers?.Find(u => u.Email == request.Email);
        createdUser.ShouldNotBeNull();

        ConfirmationEmailQueue? confirmation = await Sender.Send(new GetEmailConfirmationByUserEmail.Query(request.Email));
        confirmation.ShouldNotBeNull();
        confirmation.ConfirmationLink.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    ///   Checks that the validator passes this and the handler fails it.
    /// </summary>
    [Fact]
    public async Task DuplicateEmail_ShouldFail()
    {
        RegisterUser.Query request = new("DuplicateEmail@example.com", "abcdEFGH1!");

        // Do it
        TestValidationResult<RegisterUser.Query> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldNotHaveAnyValidationErrors();

        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.ShouldBeTrue();
        result.Result?.Succeeded.ShouldBeTrue();

        // Do it again
        TestValidationResult<RegisterUser.Query> validationResult2 = await _validator.TestValidateAsync(request);
        validationResult2.ShouldNotHaveAnyValidationErrors();

        QueryResponse<IdentityResult> result2 = await Sender.Send(request);
        result2.Success.ShouldBeFalse();
        result2.Result?.Succeeded.ShouldBeFalse();
    }
}