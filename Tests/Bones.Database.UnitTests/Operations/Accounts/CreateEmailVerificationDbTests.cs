
//using Bones.Testing.Shared.TestOperations.Users;

namespace Bones.Database.UnitTests.Operations.Accounts;

/// <summary>
///     Tests for the CreateEmailVerificationDb handler.
/// </summary>
public class CreateEmailVerificationDbTests : TestBase
{
    /// <summary>
    /// </summary>
    //[Fact]
    //public async Task ValidRequest_ShouldSuccessAndCreateDbEntry()
    //{
    //    // Setup
    //    CommandResponse createAccount = await Sender.Send(new CreateUserDb.Command("valid@example.com"));
    //    createAccount.Id.Should().NotBeNull();
    //
    //    // Test
    //    CommandResponse createEmailVerification =
    //        await Sender.Send(new CreateEmailVerification.Command(createAccount.Id!.Value));
    //    createEmailVerification.Success.Should().BeTrue();
    //    createEmailVerification.Id.Should().NotBeNull();
    //
    //    IEnumerable<UserEmailVerification> verifications =
    //        await Sender.Send(new GetEmailVerificationForUser.Query(createAccount.Id!.Value));
    //    verifications.Count().Should().Be(1);
    //}
}