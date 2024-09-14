using Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;
using Bones.Backend.Features.AccountManagement.RegisterUser;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Backend.Models;
using Bones.Testing.Shared.Backend;
using Bones.Testing.Shared.Backend.TestOperations.AccountManagement;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.UnitTests.Features.AccountManagement;

public class QueueConfirmationEmailTests : TestBase
{
    private readonly QueueConfirmationEmailCommandValidator _validator = new();

    /// <summary>
    ///     Checks that the handlers stops this.
    /// </summary>
    [Fact]
    public async Task QueueConfirmationEmail_ShouldFailWhenAlreadyInQueue()
    {
        RegisterUserQuery createUserRequest = new("ValidEmailAndPassword@example.com", "abcdEFGH1!");

        QueryResponse<IdentityResult> result = await Sender.Send(createUserRequest);
        result.Success.Should().BeTrue();
        result.Result?.Succeeded.Should().BeTrue();

        List<BonesUser>? allUsers = await Sender.Send(new GetAllUsersQuery());
        BonesUser? createdUser = allUsers?.Find(u => u.Email == createUserRequest.Email);
        createdUser.Should().NotBeNull();

        ConfirmationEmailQueue? confirmation = await Sender.Send(new GetEmailConfirmationByUserEmailQuery(createUserRequest.Email));
        confirmation.Should().NotBeNull();
        confirmation?.ConfirmationLink.Should().NotBeNullOrEmpty();

        QueueConfirmationEmailCommand confirmationEmailCommand = new(createdUser ?? throw new(), createUserRequest.Email);
        TestValidationResult<QueueConfirmationEmailCommand> validationResult = await _validator.TestValidateAsync(confirmationEmailCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse confirmationCommand = await Sender.Send(confirmationEmailCommand);
        confirmationCommand.Success.Should().BeFalse();
    }
}