using Bones.Backend.Features.AccountManagement.QueueForgotPasswordEmail;
using Bones.Backend.Features.AccountManagement.RegisterUser;
using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Backend;
using Bones.Testing.Shared.Backend.TestOperations.AccountManagement;
using FluentValidation.TestHelper;

namespace Bones.Backend.UnitTests.Features.AccountManagement;

public class QueueForgotPasswordEmailTests : TestBase
{
    private readonly QueueForgotPasswordEmailCommandValidator _validator = new();

    /// <summary>
    ///     Makes sure this works
    /// </summary>
    [Fact]
    public async Task QueueForgotPasswordEmail_ShouldPassForValidUserEmail()
    {
        RegisterUserQuery createUserRequest = new("ValidEmailAndPassword@example.com", "abcdEFGH1!");
        await Sender.Send(createUserRequest);
        await Sender.Send(new ConfirmUserByEmailCommand(createUserRequest.Email));

        QueueForgotPasswordEmailCommand forgotPasswordCommand = new(createUserRequest.Email);

        TestValidationResult<QueueForgotPasswordEmailCommand> validationResult = await _validator.TestValidateAsync(forgotPasswordCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse result = await Sender.Send(forgotPasswordCommand);
        result.Success.Should().BeTrue();

        ForgotPasswordEmailQueue? queueItem = await Sender.Send(new GetForgotPasswordQueueItemByUserEmailQuery(createUserRequest.Email));
        queueItem.Should().NotBeNull();

        if (queueItem is not null)
        {
            queueItem.EmailTo.Should().Be(createUserRequest.Email);
            queueItem.PasswordResetLink.Should().NotBeNullOrEmpty();
        }
    }

    /// <summary>
    ///     Makes sure we aren't returning any sort of error if no users exist with that email
    /// </summary>
    [Fact]
    public async Task QueueForgotPasswordEmail_ShouldPassForUnknownUserEmail()
    {
        CommandResponse result = await Sender.Send(new QueueForgotPasswordEmailCommand("UnknownEmail@example.com"));
        result.Success.Should().BeTrue();
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
        QueueForgotPasswordEmailCommand forgotPasswordCommand = new(email!);
        TestValidationResult<QueueForgotPasswordEmailCommand> validationResult = await _validator.TestValidateAsync(forgotPasswordCommand);
        validationResult.ShouldHaveValidationErrorFor(x => x.Email);
    }
}