using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.Audit;
using Bones.Database.Operations.System.SystemSettings;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.Operations.System.TestingDataSetup.Steps;

/// <inheritdoc />
public class SetupTestUserDb(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<SetupTestUserDb.Command, CommandResponse>
{
    /// <summary>
    ///   Command for setting up default system settings.
    /// </summary>
    public sealed record Command : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesUser userToCreate = new()
        {
            DisplayName = "Test User",
            UserName = DefaultValues.TEST_USER_EMAIL,
            Email = DefaultValues.TEST_USER_EMAIL,
            EmailConfirmed = true,
            EmailConfirmedDateTime = DateTimeOffset.Now,
            PasswordExpired = false
        };

        await userManager.CreateAsync(userToCreate, "Example1!");
        BonesUser? testUser = await userManager.FindByEmailAsync(DefaultValues.TEST_USER_EMAIL);
        BonesUser? backgroundServiceUser = await sender.Send(new GetBackgroundServiceUserDb.Query(), cancellationToken);

        if (testUser == null)
        {
            return CommandResponse.Fail("Test user not created");
        }

        if (backgroundServiceUser == null)
        {
            return CommandResponse.Fail("Background service user not found");
        }

        await sender.Send(new AddAccountAuditDb.Command(
            testUser,
            DbSets.Audit.AccountAudit.Actions.Create,
            backgroundServiceUser,
            "Generated"), cancellationToken);

        return CommandResponse.Pass(testUser?.Id);
    }
}