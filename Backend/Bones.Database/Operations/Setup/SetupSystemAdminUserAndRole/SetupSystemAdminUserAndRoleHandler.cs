using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.Operations.Setup.SetupSystemAdminUserAndRole;

internal sealed class SetupSystemAdminUserAndRoleHandler(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager)
    : IRequestHandler<SetupSystemAdminUserAndRoleCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(SetupSystemAdminUserAndRoleCommand request, CancellationToken cancellationToken)
    {
        // Create System Admin user
        BonesUser? createdAdminUser = await CreateAdminUserIfNotExistAsync(cancellationToken);

        // Create System Administrators role
        await CreateSysAdminRoleIfNotExistAsync(cancellationToken);

        if (createdAdminUser != null)
        {
            await userManager.AddToRoleAsync(createdAdminUser, SystemRoles.SYSTEM_ADMINISTRATORS);
            Log.Information("Added newly created SysAdmin user to SysAdmin role.");
        }

        return CommandResponse.Pass();
    }

    private async Task<BonesUser?> CreateAdminUserIfNotExistAsync(CancellationToken cancellationToken)
    {
        if (!await userManager.Users.AnyAsync(cancellationToken))
        {
            const string defaultEmail = "admin@example.com";

            BonesUser userToCreate = new()
            {
                UserName = defaultEmail,
                Email = defaultEmail,
                EmailConfirmed = true,
                EmailConfirmedDateTime = DateTimeOffset.Now,
                PasswordExpired = true
            };

            await userManager.CreateAsync(userToCreate, "ChangeMe1!");
            BonesUser? createdAdminUser = await userManager.FindByEmailAsync(defaultEmail);
            Log.Information("SysAdmin user created: {UserId}", createdAdminUser?.Id);
            return createdAdminUser;
        }
        else
        {
            Log.Information("No SysAdmin user created.");
            return null;
        }
    }

    private async Task CreateSysAdminRoleIfNotExistAsync(CancellationToken cancellationToken)
    {
        if (!await roleManager.RoleExistsAsync(SystemRoles.SYSTEM_ADMINISTRATORS))
        {
            BonesRole roleToCreate = new()
            {
                Name = SystemRoles.SYSTEM_ADMINISTRATORS,
                NormalizedName = SystemRoles.SYSTEM_ADMINISTRATORS.ToUpperInvariant(),
                IsSystemRole = true
            };

            await roleManager.CreateAsync(roleToCreate);

            BonesRole? createdRole = await roleManager.FindByNameAsync(SystemRoles.SYSTEM_ADMINISTRATORS);
            if (createdRole != null)
            {
                Claim roleClaim = new(BonesClaimTypes.Role.System.SYSTEM_ADMINISTRATOR, ClaimValues.YES);
                await roleManager.AddClaimAsync(createdRole, roleClaim);
            }
            Log.Information("SysAdmin role created.");
        }
        else
        {
            Log.Information("SysAdmin role already exists.");
        }
    }
}