using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.System;
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
    internal DbSet<AssetField> AssetFields { get; set; }
    internal DbSet<AssetFieldListEntry> AssetFieldListEntries { get; set; }
    internal DbSet<AssetLayout> AssetLayouts { get; set; }
    internal DbSet<AssetLayoutVersion> AssetLayoutVersions { get; set; }
    internal DbSet<AssetValue> AssetValues { get; set; }
    internal DbSet<AssetVersion> AssetVersions { get; set; }
    #endregion

    #region OrganizationManagement
    internal DbSet<BonesOrganization> Organizations { get; set; }
    #endregion

    #region ProjectManagement
    internal DbSet<Initiative> Initiatives { get; set; }
    internal DbSet<Tag> Tags { get; set; }
    internal DbSet<Project> Projects { get; set; }
    internal DbSet<Queue> Queues { get; set; }

    internal DbSet<WorkItem> WorkItems { get; set; }
    internal DbSet<WorkItemField> WorkItemFields { get; set; }
    internal DbSet<WorkItemFieldListEntry> WorkItemFieldListEntries { get; set; }
    internal DbSet<WorkItemLayout> WorkItemLayouts { get; set; }
    internal DbSet<WorkItemLayoutVersion> WorkItemLayoutVersions { get; set; }
    internal DbSet<WorkItemValue> WorkItemValues { get; set; }
    internal DbSet<WorkItemVersion> WorkItemVersions { get; set; }
    #endregion

    #region System


    internal DbSet<TaskError> TaskErrors { get; set; }
    #endregion

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

    public class DateTimeOffsetUtcConverter()
        : ValueConverter<DateTimeOffset, DateTimeOffset>(dto => dto.ToUniversalTime(), dto => dto);
}