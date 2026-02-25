using Bones.Database.DbSets.System.Queues;
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
    ///   Checks that the handler stops this.
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
        validationResult.ShouldHaveValidationErrors();

        CommandResponse confirmationResult = await Sender.Send(confirmationEmailCommand);
        confirmationResult.Success.ShouldBeFalse();
    }

    /// <summary>
    ///   Checks that the handler stops this.
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
    ///   Checks that the handler allows this.
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

    /// <summary>
    ///   Tests that multiple valid emails are processed in order
    /// </summary>
    [Fact]
    public async Task MultipleEmails_ShouldProcessInOrder()
    {
        const string email1 = "first@example.com";
        const string email2 = "second@example.com";
        const string email3 = "third@example.com";

        // Add emails to queue
        AddConfirmationEmailToQueueDb.Command command1 = new(email1, "http://localhost/confirm-pls");
        AddConfirmationEmailToQueueDb.Command command2 = new(email2, "http://localhost/confirm-pls");
        AddConfirmationEmailToQueueDb.Command command3 = new(email3, "http://localhost/confirm-pls");

        CommandResponse result1 = await Sender.Send(command1);
        CommandResponse result2 = await Sender.Send(command2);
        CommandResponse result3 = await Sender.Send(command3);

        result1.Success.ShouldBeTrue();
        result2.Success.ShouldBeTrue();
        result3.Success.ShouldBeTrue();

        // Verify queue order
        QueryResponse<List<ConfirmationEmailQueue>> queue = await Sender.Send(new GetConfirmationEmailsInQueueDb.Query());
        queue.Result.ShouldNotBeNull();
        queue.Result.Count.ShouldBe(3);
        queue.Result[0].EmailTo.ShouldBe(email1);
        queue.Result[1].EmailTo.ShouldBe(email2);
        queue.Result[2].EmailTo.ShouldBe(email3);
    }
}