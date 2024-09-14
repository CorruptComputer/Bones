using Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;
using Bones.Backend.Features.AccountManagement.RegisterUser;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.SystemQueues;
using Bones.Database.Operations.SystemQueues.AddConfirmationEmailToQueueDb;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Backend;
using Bones.Testing.Shared.Backend.TestOperations.AccountManagement;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.UnitTests.Operations.SystemQueues;

public class AddConfirmationEmailToQueueDbTests : TestBase
{
    private readonly AddConfirmationEmailToQueueDbCommandValidator _validator = new();

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
        AddConfirmationEmailToQueueDbCommand confirmationEmailCommand = new(email, "http://localhost/confirm-pls");
        TestValidationResult<AddConfirmationEmailToQueueDbCommand> validationResult = await _validator.TestValidateAsync(confirmationEmailCommand);
        validationResult.ShouldHaveAnyValidationError();

        CommandResponse confirmationResult = await Sender.Send(confirmationEmailCommand);
        confirmationResult.Success.Should().BeFalse();
    }

    /// <summary>
    ///     Checks that the handler stops this.
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldFailWhenAlreadyInQueue()
    {
        const string emailAddress = "DuplicateConfirmation@example.com";

        AddConfirmationEmailToQueueDbCommand confirmationEmailCommand = new(emailAddress, "http://localhost/confirm-pls");
        TestValidationResult<AddConfirmationEmailToQueueDbCommand> validationResult = await _validator.TestValidateAsync(confirmationEmailCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse confirmationResult = await Sender.Send(confirmationEmailCommand);
        confirmationResult.Success.Should().BeTrue();

        CommandResponse confirmationResult2 = await Sender.Send(confirmationEmailCommand);
        confirmationResult2.Success.Should().BeFalse();
    }

    /// <summary>
    ///     Checks that the handler stops this.
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldPass()
    {
        const string emailAddress = "IAmAValidEmail@example.com";

        AddConfirmationEmailToQueueDbCommand confirmationEmailCommand = new(emailAddress, "http://localhost/confirm-pls");
        TestValidationResult<AddConfirmationEmailToQueueDbCommand> validationResult = await _validator.TestValidateAsync(confirmationEmailCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse confirmationResult = await Sender.Send(confirmationEmailCommand);
        confirmationResult.Success.Should().BeTrue();
    }
}