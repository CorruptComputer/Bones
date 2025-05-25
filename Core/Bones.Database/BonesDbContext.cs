using System.Net;
using Bones.Database.Converters;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.MappingManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.System;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.System;
using Bones.Shared.Exceptions;
using GeoJSON.Text.Feature;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database;

/// <summary>
///     Database context for the application.
/// </summary>
/// <param name="backendConfig">Backend configuration</param>
public class BonesDbContext(BonesBackendConfiguration backendConfig)
    : IdentityDbContext<BonesUser, BonesRole, Guid, BonesUserClaim, BonesUserRole, BonesUserLogin, BonesRoleClaim, BonesUserToken>
{
    #region AccountManagement
    internal DbSet<BonesUserSession> UserSessions { get; set; }
    /// The rest are all added by the base class, however we do override the base settings in <see cref="OnModelCreating(ModelBuilder)"/>
    #endregion

    #region AssetManagement
    internal DbSet<Asset> Assets { get; set; }
    #endregion

    #region Audit
    internal DbSet<AccountAudit> AccountAudits { get; set; }

    internal DbSet<LoginAudit> LoginAudits { get; set; }

    internal DbSet<SessionAttemptAudit> SessionAttemptAudits { get; set; }

    internal DbSet<SystemAudit> SystemAudits { get; set; }
    #endregion

    #region DocumentationManagement
    // TODO: Add documentation management
    #endregion

    #region GenericItems
    internal DbSet<GenericItemField> ItemFields { get; set; }
    internal DbSet<GenericItemFieldListEntry> ItemFieldListEntries { get; set; }
    internal DbSet<GenericItemFieldVersion> ItemFieldVersions { get; set; }

    internal DbSet<GenericItemLayout> ItemLayouts { get; set; }
    internal DbSet<GenericItemLayoutVersion> ItemLayoutVersions { get; set; }

    internal DbSet<GenericItem> Items { get; set; }
    internal DbSet<GenericItemValue> ItemValues { get; set; }
    internal DbSet<GenericItemVersion> ItemVersions { get; set; }
    #endregion

    #region OrganizationManagement
    internal DbSet<BonesOrganization> Organizations { get; set; }
    #endregion

    #region ProjectManagement
    internal DbSet<Initiative> Initiatives { get; set; }
    internal DbSet<Project> Projects { get; set; }
    #endregion

    #region System
    /// See also <see cref="OnConfiguring(DbContextOptionsBuilder)" />, "__EFMigrationsHistory" is here too

    internal DbSet<ConfirmationEmailDeadQueue> ConfirmationEmailDeadQueue { get; set; }
    internal DbSet<ConfirmationEmailQueue> ConfirmationEmailQueue { get; set; }

    internal DbSet<ForgotPasswordEmailDeadQueue> ForgotPasswordEmailDeadQueue { get; set; }
    internal DbSet<ForgotPasswordEmailQueue> ForgotPasswordEmailQueue { get; set; }

    internal DbSet<SystemSetting> SystemSettings { get; set; }

    internal DbSet<TaskError> TaskErrors { get; set; }
    #endregion

    #region WorkItemManagement
    internal DbSet<WorkItemQueue> WorkItemQueues { get; set; }

    internal DbSet<WorkItem> WorkItems { get; set; }
    #endregion

    /// <summary>
    ///   Configure how certain Types are treated
    /// </summary>
    /// <param name="configurationBuilder"></param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetUtcConverter>();
        configurationBuilder.Properties<Feature>().HaveConversion<GeoJsonFeatureToStringConverter>();
        configurationBuilder.Properties<FeatureCollection>().HaveConversion<GeoJsonFeatureCollectionToStringConverter>();
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

        // Then set what we want
        builder.Entity<BonesUser>(BonesUser.BuildTable);
        builder.Entity<BonesUserRole>(BonesUserRole.BuildTable);
        builder.Entity<BonesUserLogin>(BonesUserLogin.BuildTable);
        builder.Entity<BonesUserClaim>(BonesUserClaim.BuildTable);
        builder.Entity<BonesUserToken>(BonesUserToken.BuildTable);
        builder.Entity<BonesUserSession>(BonesUserSession.BuildTable);
        builder.Entity<BonesRole>(BonesRole.BuildTable);
        builder.Entity<BonesRoleClaim>(BonesRoleClaim.BuildTable);
        builder.Entity<WorkItem>(WorkItem.BuildTable);
        builder.Entity<GenericItemLayoutFieldVersionLink>(GenericItemLayoutFieldVersionLink.BuildTable);
        builder.Entity<OsmObject>(OsmObject.BuildTable);
        builder.Entity<Asset>(Asset.BuildTable);
        builder.Entity<Initiative>(Initiative.BuildTable);
        builder.Entity<GenericItemVersion>(GenericItemVersion.BuildTable);
        builder.Entity<GenericItemField>(GenericItemField.BuildTable);
        builder.Entity<GenericItemFieldListEntry>(GenericItemFieldListEntry.BuildTable);
        builder.Entity<GenericItem>(GenericItem.BuildTable);
        builder.Entity<GenericItemValue>(GenericItemValue.BuildTable);
        builder.Entity<GeoLocation>(GeoLocation.BuildTable);
        builder.Entity<GenericItemLayoutVersion>(GenericItemLayoutVersion.BuildTable);
        builder.Entity<GenericItemFieldVersion>(GenericItemFieldVersion.BuildTable);
        builder.Entity<GenericItemLayout>(GenericItemLayout.BuildTable);
        builder.Entity<Project>(Project.BuildTable);
    }
}
