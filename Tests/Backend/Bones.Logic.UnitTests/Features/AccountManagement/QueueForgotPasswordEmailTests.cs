using Bones.Logic.Features.Accounts;
using Bones.Database.DbSets.System;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Backend;
using Bones.Testing.Shared.Backend.TestOperations.AccountManagement;
using FluentValidation.TestHelper;
using Bones.Logic.Features.System;
using Bones.Database.Operations.System.SystemSettings;

namespace Bones.Logic.UnitTests.Features.AccountManagement;

/// <summary>
///   Tests for the forgot password email queue
/// </summary>
public class QueueForgotPasswordEmailTests : TestBase
{
    private readonly QueueForgotPasswordEmail.Validator _validator = new();

    /// <summary>
    ///     Makes sure this works
    /// </summary>
    [Fact]
    public async Task QueueForgotPasswordEmail_ShouldPassForValidUserEmail()
    {
        // Set up the WebUiBaseUrl
        await Sender.Send(new SaveWebUiBaseUrlDb.Command("http://localhost:9080", "Test setup", await GetBackgroundServiceUserAsync()));

        RegisterUser.Query createUserRequest = new("ValidEmailAndPassword@example.com", "abcdEFGH1!");
        await Sender.Send(createUserRequest);
        await Sender.Send(new ConfirmUserByEmail.Command(createUserRequest.Email));

        QueueForgotPasswordEmail.Command forgotPasswordCommand = new(createUserRequest.Email);

        TestValidationResult<QueueForgotPasswordEmail.Command> validationResult = await _validator.TestValidateAsync(forgotPasswordCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse result = await Sender.Send(forgotPasswordCommand);
        result.Success.ShouldBeTrue();

        ForgotPasswordEmailQueue? queueItem = await Sender.Send(new GetForgotPasswordQueueItemByUserEmail.Query(createUserRequest.Email));
        queueItem.ShouldNotBeNull();
        queueItem.EmailTo.ShouldBe(createUserRequest.Email);
        queueItem.PasswordResetLink.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    ///     Makes sure we aren't returning any sort of error if no users exist with that email
    /// </summary>
    [Fact]
    public async Task QueueForgotPasswordEmail_ShouldPassForUnknownUserEmail()
    {
        // Set up the WebUiBaseUrl
        await Sender.Send(new SaveWebUiBaseUrlDb.Command("http://localhost:9080", "Test setup", await GetBackgroundServiceUserAsync()));

        CommandResponse result = await Sender.Send(new QueueForgotPasswordEmail.Command("UnknownEmail@example.com"));
        result.Success.ShouldBeTrue();
    }

    /// <summary>
    ///     Make sure the validator is validating the requests
    /// </summary>
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
    public async Task QueueForgotPasswordEmailValidator_ShouldReturnErrorForInvalidEmails(string? email)
    {
        QueueForgotPasswordEmail.Command forgotPasswordCommand = new(email!);
        TestValidationResult<QueueForgotPasswordEmail.Command> validationResult = await _validator.TestValidateAsync(forgotPasswordCommand);
        validationResult.ShouldHaveValidationErrorFor(x => x.Email);
    }
}