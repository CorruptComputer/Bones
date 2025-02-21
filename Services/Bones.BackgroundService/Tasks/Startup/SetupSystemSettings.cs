using System.Security.Claims;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Database.Operations.System.SystemSettings.Models;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.BackgroundService.Tasks.Startup;

internal class SetupSystemSettings(ISender sender, UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, IHostEnvironment environment) : StartupTaskBase(sender)
{
    protected override async Task RunTaskAsync(CancellationToken cancellationToken)
    {
        // This one must be first, as we will need the System Admin role for the background service user
        // We also can't create the background service user first, since this only creates the user if there are no users in the DB at all.
        await SetupInitialSystemAdminUserAndRole(cancellationToken);

        // This one must be second, as we'll need the background service to set any of the others
        BonesUser backgroundServiceUser = await SetupBackgroundServiceUser(cancellationToken);

        // Any other system settings that need a default can be added after those, since we'll need the background service user for them
        await SetupWebUiBaseUrl(backgroundServiceUser, cancellationToken);
        await SetupStmpConfig(backgroundServiceUser, cancellationToken);
    }

    #region SetupInitialSystemAdminUserAndRole
    private async Task SetupInitialSystemAdminUserAndRole(CancellationToken cancellationToken)
    {
        // Create the System Admin user
        BonesUser? adminUser = await CreateAdminUserIfNotExistAsync(cancellationToken);

        // Create System Administrators role
        await CreateSysAdminRoleIfNotExistAsync();

        if (adminUser != null)
        {
            await userManager.AddToRoleAsync(adminUser, SystemRoles.SYSTEM_ADMINISTRATORS);
            Log.Information("Added newly created SysAdmin user to SysAdmin role.");
        }
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

    private async Task CreateSysAdminRoleIfNotExistAsync()
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
    #endregion

    private async Task<BonesUser> SetupBackgroundServiceUser(CancellationToken cancellationToken) 
    {
        BonesUser? user = await Sender.Send(new GetBackgroundServiceUserDb.Query(), cancellationToken);

        if (user == null)
        {
            const string defaultEmail = "no-reply@example.com";

            BonesUser userToCreate = new()
            {
                UserName = defaultEmail,
                Email = defaultEmail,
                EmailConfirmed = true,
                EmailConfirmedDateTime = DateTimeOffset.Now,
                PasswordExpired = false
            };

            await userManager.CreateAsync(userToCreate);
            
            user = await userManager.FindByEmailAsync(defaultEmail)
                ?? throw new BonesException("Failed to find the Background Service user after creation.");

            await userManager.AddToRoleAsync(user, SystemRoles.SYSTEM_ADMINISTRATORS);

            await Sender.Send(new SaveBackgroundServiceUserIdDb.Command(user.Id, AuditActionReasons.DbSetup, user), cancellationToken);

            Log.Information("Background Service user created: {UserId}", user.Id);
        }
        
        return user;
    }

    private async Task SetupWebUiBaseUrl(BonesUser backgroundServiceUser, CancellationToken cancellationToken)
    {
        string? webUiBaseUrl = await Sender.Send(new GetWebUiBaseUrlDb.Query(), cancellationToken);

        // In local environment want to create a default config and send emails to mailhog
        if (string.IsNullOrEmpty(webUiBaseUrl) && environment.IsDevelopment())
        {
            webUiBaseUrl = "http://localhost:9080";

            await Sender.Send(new SaveWebUiBaseUrlDb.Command(webUiBaseUrl, AuditActionReasons.DbSetup, backgroundServiceUser), cancellationToken);
            Log.Information("Web UI Base URL set to '{WebUIBaseURL}' for local environment.", webUiBaseUrl);
        }
    }

    private async Task SetupStmpConfig(BonesUser backgroundServiceUser, CancellationToken cancellationToken)
    {
        SmtpConfig? smtpConfig = await Sender.Send(new GetSmtpConfigDb.Query(), cancellationToken);

        // In local environment want to create a default config and send emails to mailhog
        if (smtpConfig == null && environment.IsDevelopment())
        {
            smtpConfig = new SmtpConfig
            {
                Server = "localhost",
                Port = 1025,
                UseSsl = false,
            };

            await Sender.Send(new SaveSmtpConfigDb.Command(smtpConfig, AuditActionReasons.DbSetup, backgroundServiceUser), cancellationToken);
            Log.Information("SMTP Config created for local environment.");
        }

    }
}
