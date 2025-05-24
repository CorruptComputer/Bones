using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.System;
using Bones.Shared.Backend.Models;
using Bones.Testing.UnitTests.Shared;
using Bones.Testing.UnitTests.Shared.TestOperations.AccountManagement;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using Bones.Logic.Features.System;
using Bones.Logic.Features.Accounts;
using Bones.Database.Operations.System.SystemSettings;

namespace Bones.Logic.UnitTests.Features.AccountManagement;

/// <summary>
///   Tests for the confirmation email queue
/// </summary>
public class QueueConfirmationEmailTests : TestBase
{
    private readonly QueueConfirmationEmail.Validator _validator = new();

    /// <summary>
    ///   Checks that the handlers stops this.
    /// </summary>
    [Fact]
    public async Task QueueConfirmationEmail_ShouldFailWhenAlreadyInQueue()
    {
        // Set up the WebUiBaseUrl
        await Sender.Send(new SaveWebUiBaseUrlDb.Command("http://localhost:9080", "Test setup", await GetBackgroundServiceUserAsync()));

        RegisterUser.Query createUserRequest = new("ValidEmailAndPassword@example.com", "abcdEFGH1!");

        QueryResponse<IdentityResult> result = await Sender.Send(createUserRequest);
        result.Success.ShouldBeTrue();
        result.Result?.Succeeded.ShouldBeTrue();

        List<BonesUser>? allUsers = await Sender.Send(new GetAllUsers.Query());
        BonesUser? createdUser = allUsers?.Find(u => u.Email == createUserRequest.Email);
        createdUser.ShouldNotBeNull();

        ConfirmationEmailQueue? confirmation = await Sender.Send(new GetEmailConfirmationByUserEmail.Query(createUserRequest.Email));
        confirmation.ShouldNotBeNull();
        confirmation.ConfirmationLink.ShouldNotBeNullOrEmpty();

        QueueConfirmationEmail.Command confirmationEmailCommand = new(createdUser, createUserRequest.Email);
        TestValidationResult<QueueConfirmationEmail.Command> validationResult = await _validator.TestValidateAsync(confirmationEmailCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse confirmationCommand = await Sender.Send(confirmationEmailCommand);
        confirmationCommand.Success.ShouldBeFalse();
    }
}