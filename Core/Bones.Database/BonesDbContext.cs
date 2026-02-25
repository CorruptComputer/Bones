using System.Net;
using Bones.Database.Converters;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Audits;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.Items.Assignments;
using Bones.Database.DbSets.Items.Fields;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.DbSets.Items.Types;
using Bones.Database.DbSets.Organizations;
using Bones.Database.DbSets.Projects;
using Bones.Database.DbSets.System;
using Bones.Database.DbSets.System.Queues;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database;

/// <summary>
///   Database context for the application.
/// </summary>
/// <param name="backendConfig">Backend configuration</param>
public class BonesDbContext(BonesBackendConfiguration backendConfig)
    : IdentityDbContext<BonesUser, BonesRole, Guid, BonesUserClaim, BonesUserRole, BonesUserLogin, BonesRoleClaim, BonesUserToken>
{
    #region Accounts
    internal DbSet<BonesUserSession> UserSessions { get; set; }
    /// The rest are all added by the base class, however we do override the base settings in <see cref="OnModelCreating(ModelBuilder)"/>
    #endregion

    #region Audits
    internal DbSet<AccountAudit> AccountAudits { get; set; }

    internal DbSet<LoginAudit> LoginAudits { get; set; }

    internal DbSet<SessionAttemptAudit> SessionAttemptAudits { get; set; }

    internal DbSet<SystemAudit> SystemAudits { get; set; }
    #endregion

    #region Items
    #region Assignment
    internal DbSet<ItemAssignee> ItemAssignees { get; set; }
    internal DbSet<ItemAssignmentSlot> ItemAssignmentSlots { get; set; }
    #endregion

    #region Fields
    internal DbSet<ItemField> ItemFields { get; set; }
    internal DbSet<ItemFieldListEntry> ItemFieldListEntries { get; set; }
    internal DbSet<ItemFieldVersion> ItemFieldVersions { get; set; }
    #endregion

    #region Layouts
    internal DbSet<ItemLayout> ItemLayouts { get; set; }
    internal DbSet<ItemLayoutFieldVersionLink> ItemLayoutFieldVersionLinks { get; set; }
    internal DbSet<ItemLayoutVersion> ItemLayoutVersions { get; set; }
    #endregion

    #region Types
    internal DbSet<Asset> Assets { get; set; }
    internal DbSet<BonesTask> Tasks { get; set; }
    #endregion

    internal DbSet<Item> Items { get; set; }
    internal DbSet<ItemValue> ItemValues { get; set; }
    internal DbSet<ItemVersion> ItemVersions { get; set; }
    #endregion

    #region Organizations
    internal DbSet<BonesOrganization> Organizations { get; set; }
    #endregion

    #region Projects
    internal DbSet<Initiative> Initiatives { get; set; }
    internal DbSet<Project> Projects { get; set; }
    internal DbSet<TaskQueue> TaskQueues { get; set; }
    #endregion

    #region System
    /// See also <see cref="OnConfiguring(DbContextOptionsBuilder)" />, "__EFMigrationsHistory" is here too

    #region Queues
    internal DbSet<ConfirmationEmailDeadQueue> ConfirmationEmailDeadQueue { get; set; }
    internal DbSet<ConfirmationEmailQueue> ConfirmationEmailQueue { get; set; }

    internal DbSet<ForgotPasswordEmailDeadQueue> ForgotPasswordEmailDeadQueue { get; set; }
    internal DbSet<ForgotPasswordEmailQueue> ForgotPasswordEmailQueue { get; set; }
    #endregion

    internal DbSet<SystemSetting> SystemSettings { get; set; }

    internal DbSet<TaskError> TaskErrors { get; set; }
    #endregion

    /// <summary>
    ///   Configure how certain Types are treated
    /// </summary>
    /// <param name="configurationBuilder"></param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetUtcConverter>();
        configurationBuilder.Properties<IPAddress>().HaveConversion<IPAddressToStringConverter>();
    }

    /// <summary>
    ///   Configure which database to use: PostgreSQL in most cases, in-memory DB for unit tests.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <exception cref="BonesException"></exception>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Should only be used for unit/integration testing
        if (backendConfig.UseInMemoryDb)
        {
            Log.Warning("Using in-memory database, this should only be used for unit/integration tests.");

            Guid inMemoryDbId = backendConfig.InMemoryDbId ?? Guid.NewGuid();

            // Name needs to be unique, else the tests will clobber each other
            optionsBuilder.UseInMemoryDatabase($"BonesInMemoryDb-{inMemoryDbId}",
                options =>
                {
                    options.EnableNullChecks();
                }
            );
        }
        else
        {
            if (string.IsNullOrWhiteSpace(backendConfig.DatabaseConnectionString))
            {
                throw new BonesException("BonesBackendConfiguration:DatabaseConnectionString is missing.");
            }

            optionsBuilder.UseNpgsql(backendConfig.DatabaseConnectionString,
                options =>
                {
                    options.MigrationsHistoryTable("__EFMigrationsHistory", "System");
                    options.MigrationsAssembly(typeof(BonesDbContext).Assembly.FullName);
                    options.EnableRetryOnFailure();
                }
            );
        }
    }

    /// <summary>
    ///   Create models
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Go ahead and let the base class do its thing
        base.OnModelCreating(builder);

        // Account management, most of these are what we're overriding from the base
        builder.Entity<BonesRole>(BonesRole.BuildTable);
        builder.Entity<BonesRoleClaim>(BonesRoleClaim.BuildTable);
        builder.Entity<BonesUser>(BonesUser.BuildTable);
        builder.Entity<BonesUserClaim>(BonesUserClaim.BuildTable);
        builder.Entity<BonesUserLogin>(BonesUserLogin.BuildTable);
        builder.Entity<BonesUserRole>(BonesUserRole.BuildTable);
        builder.Entity<BonesUserSession>(BonesUserSession.BuildTable);
        builder.Entity<BonesUserToken>(BonesUserToken.BuildTable);

        // Asset Management
        builder.Entity<Asset>(Asset.BuildTable);

        // Audit
        builder.Entity<AccountAudit>(AccountAudit.BuildTable);
        builder.Entity<LoginAudit>(LoginAudit.BuildTable);
        builder.Entity<SessionAttemptAudit>(SessionAttemptAudit.BuildTable);
        builder.Entity<SystemAudit>(SystemAudit.BuildTable);

        // Items
        builder.Entity<Item>(Item.BuildTable);
        builder.Entity<ItemAssignee>(ItemAssignee.BuildTable);
        builder.Entity<ItemAssignmentSlot>(ItemAssignmentSlot.BuildTable);
        builder.Entity<ItemField>(ItemField.BuildTable);
        builder.Entity<ItemFieldListEntry>(ItemFieldListEntry.BuildTable);
        builder.Entity<ItemFieldVersion>(ItemFieldVersion.BuildTable);
        builder.Entity<ItemLayout>(ItemLayout.BuildTable);
        builder.Entity<ItemLayoutFieldVersionLink>(ItemLayoutFieldVersionLink.BuildTable);
        builder.Entity<ItemLayoutVersion>(ItemLayoutVersion.BuildTable);
        builder.Entity<ItemValue>(ItemValue.BuildTable);
        builder.Entity<ItemVersion>(ItemVersion.BuildTable);

        // Organization Management
        builder.Entity<BonesOrganization>(BonesOrganization.BuildTable);

        // Project Management
        builder.Entity<Initiative>(Initiative.BuildTable);
        builder.Entity<Project>(Project.BuildTable);

        // System
        //builder.Entity<ConfirmationEmailDeadQueue>(ConfirmationEmailDeadQueue.BuildTable);
        //builder.Entity<ConfirmationEmailQueue>(ConfirmationEmailQueue.BuildTable);
        //builder.Entity<ForgotPasswordEmailDeadQueue>(ForgotPasswordEmailDeadQueue.BuildTable);
        //builder.Entity<ForgotPasswordEmailQueue>(ForgotPasswordEmailQueue.BuildTable);
        //builder.Entity<SystemSetting>(SystemSetting.BuildTable);
        //builder.Entity<TaskError>(TaskError.BuildTable);

        // Task Management
        builder.Entity<BonesTask>(BonesTask.BuildTable);
        builder.Entity<TaskQueue>(TaskQueue.BuildTable);
    }
}
