using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.GenericItems.ItemFields;
using Bones.Database.DbSets.GenericItems.ItemLayouts;
using Bones.Database.DbSets.GenericItems.Items;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.System;
using Bones.Database.DbSets.SystemQueues;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Models;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database;

/// <summary>
///     Database context for the application.
/// </summary>
/// <param name="dbConfig">Database configuration</param>
public class BonesDbContext(DatabaseConfiguration dbConfig)
    : IdentityDbContext<BonesUser, BonesRole, Guid, BonesUserClaim, BonesUserRole, BonesUserLogin, BonesRoleClaim, BonesUserToken>
{
    #region AccountManagement
    // These are all added by the base class
    #endregion

    #region AssetManagement
    internal DbSet<Asset> Assets { get; set; }
    #endregion

    #region DocumentationManagement
    // TODO
    #endregion

    #region GenericItems
    internal DbSet<ItemField> ItemFields { get; set; }
    internal DbSet<ItemFieldListEntry> ItemFieldListEntries { get; set; }

    internal DbSet<ItemLayout> ItemLayouts { get; set; }
    internal DbSet<ItemLayoutVersion> ItemLayoutVersions { get; set; }

    internal DbSet<Item> Items { get; set; }
    internal DbSet<ItemValue> ItemValues { get; set; }
    internal DbSet<ItemVersion> ItemVersions { get; set; }
    #endregion

    #region OrganizationManagement
    internal DbSet<BonesOrganization> Organizations { get; set; }
    #endregion

    #region ProjectManagement
    internal DbSet<Initiative> Initiatives { get; set; }
    internal DbSet<Project> Projects { get; set; }
    #endregion

    #region System
    // See also this.OnConfiguring(), "__EFMigrationsHistory" is here too
    internal DbSet<TaskError> TaskErrors { get; set; }
    #endregion

    #region SystemQueues
    internal DbSet<ConfirmationEmailDeadQueue> ConfirmationEmailDeadQueue { get; set; }
    internal DbSet<ConfirmationEmailQueue> ConfirmationEmailQueue { get; set; }

    internal DbSet<ForgotPasswordEmailDeadQueue> ForgotPasswordEmailDeadQueue { get; set; }
    internal DbSet<ForgotPasswordEmailQueue> ForgotPasswordEmailQueue { get; set; }
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
        base.OnModelCreating(builder);

        // Want to override these to change the name, every other table should have it set via Attributes though.
        const string accountManagement = "AccountManagement";
        builder.Entity<BonesUser>().ToTable("BonesUsers", accountManagement);
        builder.Entity<BonesUserRole>().ToTable("BonesUserRoles", accountManagement);
        builder.Entity<BonesUserLogin>().ToTable("BonesUserLogins", accountManagement);
        builder.Entity<BonesUserClaim>().ToTable("BonesUserClaims", accountManagement);
        builder.Entity<BonesUserToken>().ToTable("BonesUserTokens", accountManagement);
        builder.Entity<BonesRole>().ToTable("BonesRoles", accountManagement);
        builder.Entity<BonesRoleClaim>().ToTable("BonesRoleClaims", accountManagement);
    }

    internal class DateTimeOffsetUtcConverter()
        : ValueConverter<DateTimeOffset, DateTimeOffset>(dto => dto.ToUniversalTime(), dto => dto);
}