using Bones.Database.Converters;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.System;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Models;
using Bones.Shared.Exceptions;
using GeoJSON.Text.Feature;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bones.Database;

/// <summary>
///     Database context for the application.
/// </summary>
/// <param name="dbConfig">Database configuration</param>
public class BonesDbContext(DatabaseConfiguration dbConfig)
    : IdentityDbContext<BonesUser, BonesRole, Guid, BonesUserClaim, BonesUserRole, BonesUserLogin, BonesRoleClaim, BonesUserToken>
{
    #region AccountManagement
    /// These are all added by the base class, however we do override the base settings in <see cref="OnModelCreating(ModelBuilder)"/>
    #endregion

    #region AssetManagement
    internal DbSet<Asset> Assets { get; set; }
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
    }

    /// <summary>
    ///     Configure which database to use: PostgreSQL in most cases, in-memory DB for unit tests.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <exception cref="BonesException"></exception>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Should only be used for unit testing
        if (dbConfig.UseInMemoryDb ?? false)
        {
            // Name needs to be unique, else the tests will clobber each other
            optionsBuilder.UseInMemoryDatabase($"BonesInMemoryDb-{Guid.NewGuid()}",
                options => { options.EnableNullChecks(); });
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dbConfig.ConnectionString))
            {
                throw new BonesException("DatabaseConfiguration:ConnectionString is missing.");
            }

            optionsBuilder.UseNpgsql(dbConfig.ConnectionString, options =>
            {
                options.MigrationsHistoryTable("__EFMigrationsHistory", "System");
                options.MigrationsAssembly(typeof(BonesDbContext).Assembly.FullName);
                options.EnableRetryOnFailure();
            });
        }
    }

    /// <summary>
    ///     Create models
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Go ahead and let the base class do its thing
        base.OnModelCreating(builder);

        // Want to override these to change the names and schemas that the base gives them,
        // every other table should just have it set via attributes.
        builder.Entity<BonesUser>(BonesUser.BuildTable);
        builder.Entity<BonesUserRole>(BonesUserRole.BuildTable);
        builder.Entity<BonesUserLogin>(BonesUserLogin.BuildTable);
        builder.Entity<BonesUserClaim>(BonesUserClaim.BuildTable);
        builder.Entity<BonesUserToken>(BonesUserToken.BuildTable);
        builder.Entity<BonesRole>(BonesRole.BuildTable);
        builder.Entity<BonesRoleClaim>(BonesRoleClaim.BuildTable);
    }
}
