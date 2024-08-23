using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using ItemField = Bones.Database.DbSets.ProjectManagement.ItemField;
using ItemFieldListEntry = Bones.Database.DbSets.ProjectManagement.ItemFieldListEntry;
using ItemLayout = Bones.Database.DbSets.ProjectManagement.ItemLayout;
using ItemLayoutVersion = Bones.Database.DbSets.ProjectManagement.ItemLayoutVersion;
using ItemValue = Bones.Database.DbSets.ProjectManagement.ItemValue;
using ItemVersion = Bones.Database.DbSets.ProjectManagement.ItemVersion;

namespace Bones.Database;

/// <summary>
///     Database context for the application.
/// </summary>
/// <param name="configuration">App configuration</param>
public class BonesDbContext(IConfiguration configuration)
    : IdentityDbContext<BonesUser, BonesRole, Guid, BonesUserClaim, BonesUserRole, BonesUserLogin, BonesRoleClaim, BonesUserToken>
{
    private const string _bonesDbConnectionStringKey = "BonesDb";
    
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
    internal DbSet<Item> Items { get; set; }
    internal DbSet<ItemField> ItemFields { get; set; }
    internal DbSet<ItemFieldListEntry> ItemFieldListEntries { get; set; }
    internal DbSet<ItemLayout> ItemLayouts { get; set; }
    internal DbSet<ItemLayoutVersion> ItemLayoutVersions { get; set; }
    internal DbSet<ItemValue> ItemValues { get; set; }
    internal DbSet<ItemVersion> ItemVersions { get; set; }
    internal DbSet<Tag> Tags { get; set; }
    internal DbSet<Project> Projects { get; set; }
    internal DbSet<Queue> Queues { get; set; }
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
        string useInMemoryDbStr = configuration["Database:UseInMemoryDb"] ?? "false";
        bool useInMemoryDb = bool.Parse(useInMemoryDbStr);

        // Should only be used for unit testing
        if (useInMemoryDb)
        {
            // Name needs to be unique, else the tests will clobber each other
            optionsBuilder.UseInMemoryDatabase($"BonesInMemoryDb-{Guid.NewGuid()}",
                options => { options.EnableNullChecks(); });
        }
        else
        {
            string? connectionString = configuration.GetConnectionString(_bonesDbConnectionStringKey);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new BonesException("Missing appsettings configuration for connection string: BonesDb");
            }
            
            optionsBuilder.UseNpgsql(connectionString, options =>
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