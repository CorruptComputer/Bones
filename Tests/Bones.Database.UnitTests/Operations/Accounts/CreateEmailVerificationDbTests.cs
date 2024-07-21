using Bones.Api.Features.Accounts;
using Bones.Database.DbSets;
using Bones.Database.Operations.Accounts;
using Bones.Testing.Shared.TestOperations.Accounts;

namespace Bones.Database.UnitTests.Operations.Accounts;

/// <summary>
///     Tests for the CreateEmailVerificationDb handler.
/// </summary>
public class CreateEmailVerificationDbTests : TestBase
{
    /// <summary>
    /// </summary>
    [Fact]
    public async Task ValidRequest_ShouldSuccessAndCreateDbEntry()
    {
        // Setup
        CommandResponse createAccount = await Sender.Send(new CreateAccountDbCommand("valid@example.com"));
        createAccount.Id.Should().NotBeNull();

        // Test
        CommandResponse createEmailVerification =
            await Sender.Send(new CreateEmailVerificationCommand(createAccount.Id!.Value));
        createEmailVerification.Success.Should().BeTrue();
        createEmailVerification.Id.Should().NotBeNull();

        IEnumerable<AccountEmailVerification> verifications =
            await Sender.Send(new GetEmailVerificationForAccountQuery(createAccount.Id!.Value));
        verifications.Count().Should().Be(1);
    }
}