using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;
using ClaimTypes = Bones.Shared.Consts.ClaimTypes;

namespace Bones.Database.Operations.Setup;

public sealed class SetupSystemAdminUserAndRoleHandler(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager) : IRequestHandler<SetupSystemAdminUserAndRoleHandler.Command, CommandResponse>
{
    public sealed record Command : IRequest<CommandResponse>;

    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        // Create System Admin user
        BonesUser? createdAdminUser = null;
        if (!await userManager.Users.AnyAsync(cancellationToken))
        {
            const string defaultEmail = "admin@example.com";
            const string defaultPassword = "ChangeMe1!";
            
            BonesUser userToCreate = new()
            {
                UserName = defaultEmail,
                Email = defaultEmail,
                EmailConfirmed = true,
                EmailConfirmedDateTime = DateTimeOffset.Now,
                PasswordExpired = true
            };
            
            await userManager.CreateAsync(userToCreate, defaultPassword);
            createdAdminUser = await userManager.FindByEmailAsync(defaultEmail);
        }
        
        // Create System Administrators role
        if (!await roleManager.RoleExistsAsync(SystemRoles.SYSTEM_ADMINISTRATORS))
        {
            BonesRole roleToCreate = new()
            {
                Name = SystemRoles.SYSTEM_ADMINISTRATORS, NormalizedName = SystemRoles.SYSTEM_ADMINISTRATORS.ToUpperInvariant(), IsSystemRole = true
            };

            await roleManager.CreateAsync(roleToCreate);

            BonesRole? createdRole = await roleManager.FindByNameAsync(SystemRoles.SYSTEM_ADMINISTRATORS);
            if (createdRole != null)
            {
                Claim roleClaim = new(ClaimTypes.Role.System.SYSTEM_ADMINISTRATOR, ClaimValues.YES);
                await roleManager.AddClaimAsync(createdRole, roleClaim);
            }
        }
        
        if (createdAdminUser != null)
        {
            await userManager.AddToRoleAsync(createdAdminUser, SystemRoles.SYSTEM_ADMINISTRATORS);
        }

        return new()
        {
            Success = true
        };
    }
}