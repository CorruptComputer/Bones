using Bones.Database.Operations.System.Queues;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.SystemQueues;

/// <summary>
///   Tests for the confirmation email queue
/// </summary>
public class AddConfirmationEmailToQueueDbTests : TestBase
{
    private readonly AddConfirmationEmailToQueueDb.Validator _validator = new();

    /// <summary>
    ///     Checks that the handler stops this.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(" @ ")]
    [InlineData("InvalidEmail@example,com")]
    [InlineData("InvalidEmail@example.")]
    [InlineData("InvalidEmail@example")]
    [InlineData("InvalidEmail@")]
    [InlineData("InvalidEmail")]
    public async Task InvalidEmail_ShouldFail(string email)
    {
        AddConfirmationEmailToQueueDb.Command confirmationEmailCommand = new(email, "http://localhost/confirm-pls");
        TestValidationResult<AddConfirmationEmailToQueueDb.Command> validationResult = await _validator.TestValidateAsync(confirmationEmailCommand);
        validationResult.ShouldHaveAnyValidationError();

        CommandResponse confirmationResult = await Sender.Send(confirmationEmailCommand);
        confirmationResult.Success.ShouldBeFalse();
    }

    /// <summary>
    ///     Checks that the handler stops this.
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldFailWhenAlreadyInQueue()
    {
        const string emailAddress = "DuplicateConfirmation@example.com";

        AddConfirmationEmailToQueueDb.Command confirmationEmailCommand = new(emailAddress, "http://localhost/confirm-pls");
        TestValidationResult<AddConfirmationEmailToQueueDb.Command> validationResult = await _validator.TestValidateAsync(confirmationEmailCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse confirmationResult = await Sender.Send(confirmationEmailCommand);
        confirmationResult.Success.ShouldBeTrue();

        CommandResponse confirmationResult2 = await Sender.Send(confirmationEmailCommand);
        confirmationResult2.Success.ShouldBeFalse();
    }

    /// <summary>
    ///     Checks that the handler stops this.
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldPass()
    {
        const string emailAddress = "IAmAValidEmail@example.com";

        AddConfirmationEmailToQueueDb.Command confirmationEmailCommand = new(emailAddress, "http://localhost/confirm-pls");
        TestValidationResult<AddConfirmationEmailToQueueDb.Command> validationResult = await _validator.TestValidateAsync(confirmationEmailCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse confirmationResult = await Sender.Send(confirmationEmailCommand);
        confirmationResult.Success.ShouldBeTrue();
    }
}