using Bones.Backend.Features.Accounts;
using Bones.UnitTests.Shared.TestOperations.Accounts;

namespace Bones.Backend.UnitTests.Features.Accounts;

/// <summary>
///   Tests for the CreateAccount handler.
/// </summary>
public class CreateAccountTests : TestBase
{
    /// <summary>
    ///   Valid emails should have the account created.
    /// </summary>
    /// <param name="email">The email address to test.</param>
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("unit+test@example.com")]
    [InlineData("🍕@example.com")]
    [InlineData("_test@example.com")]
    public async Task ValidEmail_ShouldSuccessAndCreateEmailVerification(string email)
    {
        CommandResponse createAccount = await Sender.Send(new CreateAccountCommand(email));

        createAccount.FailureReason.Should().BeNull();
        createAccount.Success.Should().BeTrue();
        createAccount.Id.Should().Be(1);

        IEnumerable<GetEmailVerificationForAccountResponse> verifications = await Sender.Send(new GetEmailVerificationForAccountQuery(createAccount.Id!.Value));

        verifications.Count().Should().Be(1);
    }

    /// <summary>
    ///   Invalid emails should not have the account created.
    /// </summary>
    /// <param name="email">The email address to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(" @ ")]
    [InlineData("test@example,com")]
    [InlineData("test@example.")]
    [InlineData("test@example")]
    [InlineData("test@")]
    [InlineData("test")]
    public async Task InvalidEmail_ShouldFail(string email)
    {
        CommandResponse createAccount = await Sender.Send(new CreateAccountCommand(email));

        createAccount.Success.Should().BeFalse();
    }

    /// <summary>
    ///   Duplicate emails should not allow the account to be created.
    /// </summary>
    [Fact]
    public async Task DuplicateEmail_ShouldFail()
    {
        const string duplicateEmail = "duplicate@example.com";
        CommandResponse createAccount = await Sender.Send(new CreateAccountCommand(duplicateEmail));
        createAccount.Success.Should().BeTrue();

        CommandResponse duplicateAccount = await Sender.Send(new CreateAccountCommand(duplicateEmail));
        duplicateAccount.Success.Should().BeFalse();
    }
}