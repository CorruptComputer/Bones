using System.Security.Claims;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Audits;
using Bones.Database.Operations.Audits;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Database.Operations.System.SystemSettings.Models;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace Bones.Database.Operations.System;

/// <inheritdoc />
public class SetupDb(ISender sender, UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager,
                     IHostEnvironment environment)
    : IRequestHandler<SetupDb.Command, CommandResponse>
{
    /// <summary>
    ///   Command for setting up default system settings.
    /// </summary>
    public sealed record Command : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        await CreateSystemAdminRoleIfNotExistsAsync();
        BonesUser backgroundServiceUser = await GetOrCreateBackgroundServiceUserAsync(cancellationToken);

        await CreateAdminUserIfNoneExistAsync(backgroundServiceUser, cancellationToken);
        await SetupWebUiBaseUrl(backgroundServiceUser, cancellationToken);
        await SetupStmpConfig(backgroundServiceUser, cancellationToken);

        return CommandResponse.Pass();
    }

    private async Task CreateSystemAdminRoleIfNotExistsAsync()
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

            BonesRole? sysAdminRole = await roleManager.FindByNameAsync(SystemRoles.SYSTEM_ADMINISTRATORS);
            if (sysAdminRole != null)
            {
                Claim roleClaim = new(BonesClaimTypes.Role.System.SYSTEM_ADMINISTRATOR, ClaimValues.YES);
                await roleManager.AddClaimAsync(sysAdminRole, roleClaim);
            }

            Log.Information("SysAdmin role created.");
        }

        return;
    }

    private async Task<BonesUser> GetOrCreateBackgroundServiceUserAsync(CancellationToken cancellationToken)
    {
        BonesUser? user = await sender.Send(new GetBackgroundServiceUserDb.Query(), cancellationToken);

        if (user == null)
        {
            const string defaultEmail = "no-reply@example.com";

            BonesUser userToCreate = new()
            {
                DisplayName = "System",
                UserName = defaultEmail,
                Email = defaultEmail,
                EmailConfirmed = true,
                EmailConfirmedDateTime = DateTimeOffset.Now,
                PasswordExpired = true // This user should never need to login
            };

            await userManager.CreateAsync(userToCreate);

            user = await userManager.FindByEmailAsync(defaultEmail);
            BonesException.ThrowIfNull(user);

            await userManager.AddToRoleAsync(user, SystemRoles.SYSTEM_ADMINISTRATORS);

            await sender.Send(new AddAccountAuditDb.Command(user, AccountAudit.Actions.Create, user, AuditActionReasons.DbSetup), cancellationToken);
            await sender.Send(new SaveBackgroundServiceUserIdDb.Command(user.Id, AuditActionReasons.DbSetup, user), cancellationToken);

            Log.Information("Background Service user created: {UserId}", user.Id);
        }

        return user;
    }

    private async Task CreateAdminUserIfNoneExistAsync(BonesUser backgroundServiceUser, CancellationToken cancellationToken)
    {
        IEnumerable<BonesUser> adminUsers = await userManager.GetUsersInRoleAsync(SystemRoles.SYSTEM_ADMINISTRATORS);
        bool shouldCreate = false;

        // If the only admin user in the system is the background service user, then create a default admin user to login with
        if (!adminUsers.Any() || (adminUsers.Count() == 1 && adminUsers.First().Id == backgroundServiceUser.Id))
        {
            shouldCreate = true;
        }

        if (shouldCreate)
        {
            BonesUser userToCreate = new()
            {
                DisplayName = "Administrator",
                UserName = DefaultValues.DEFAULT_SYSTEM_ADMIN_EMAIL,
                Email = DefaultValues.DEFAULT_SYSTEM_ADMIN_EMAIL,
                EmailConfirmed = true,
                EmailConfirmedDateTime = DateTimeOffset.Now,
                PasswordExpired = false
            };

            await userManager.CreateAsync(userToCreate, "ChangeMe1!");
            BonesUser? createdAdminUser = await userManager.FindByEmailAsync(DefaultValues.DEFAULT_SYSTEM_ADMIN_EMAIL);
            if (createdAdminUser == null)
            {
                return;
            }

            await userManager.AddToRoleAsync(createdAdminUser, SystemRoles.SYSTEM_ADMINISTRATORS);
            await sender.Send(new AddAccountAuditDb.Command(createdAdminUser, AccountAudit.Actions.Create, backgroundServiceUser, AuditActionReasons.DbSetup), cancellationToken);

            Log.Information("Default SysAdmin user created: {UserId}", createdAdminUser.Id);
        }
    }

    private async Task SetupWebUiBaseUrl(BonesUser backgroundServiceUser, CancellationToken cancellationToken)
    {
        string? webUiBaseUrl = await sender.Send(new GetWebUiBaseUrlDb.Query(), cancellationToken);

        // In local environment want to create a default config and send emails to mailhog
        if (string.IsNullOrEmpty(webUiBaseUrl) && environment.IsDevelopment())
        {
            webUiBaseUrl = "http://localhost:9080";

            await sender.Send(new SaveWebUiBaseUrlDb.Command(webUiBaseUrl, AuditActionReasons.DbSetup, backgroundServiceUser), cancellationToken);
            Log.Information("Web UI Base URL set to '{WebUIBaseURL}' for local environment.", webUiBaseUrl);
        }
    }

    private async Task SetupStmpConfig(BonesUser backgroundServiceUser, CancellationToken cancellationToken)
    {
        SmtpConfig? smtpConfig = await sender.Send(new GetSmtpConfigDb.Query(), cancellationToken);

        // In local environment want to create a default config and send emails to mailhog
        if (smtpConfig == null && environment.IsDevelopment())
        {
            smtpConfig = new SmtpConfig
            {
                Server = "localhost",
                Port = 1025,
                UseSsl = false,
            };

            await sender.Send(new SaveSmtpEnabledDb.Command(true, AuditActionReasons.DbSetup, backgroundServiceUser), cancellationToken);
            await sender.Send(new SaveSmtpConfigDb.Command(smtpConfig, AuditActionReasons.DbSetup, backgroundServiceUser), cancellationToken);
            Log.Information("SMTP Config created for local environment.");
        }
    }
}
