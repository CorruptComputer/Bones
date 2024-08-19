using Bones.Database.DbSets.Identity;
using Bones.Database.DbSets.ProjectManagement.Initiatives;
using Bones.Database.DbSets.ProjectManagement.Items;
using Bones.Database.DbSets.ProjectManagement.Layouts;
using Bones.Database.DbSets.ProjectManagement.Projects;
using Bones.Database.DbSets.ProjectManagement.Queues;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Bones.Database;

/// <summary>
///     Database context for the application.
/// </summary>
/// <param name="configuration">App configuration</param>
public class BonesDbContext(IConfiguration configuration)
    : IdentityDbContext<BonesUser, BonesRole, Guid, BonesUserClaim, BonesUserRole, BonesUserLogin, BonesRoleClaim, BonesUserToken>
{
    private const string BonesDbConnectionStringKey = "BonesDb";
    
    #region ProjectManagement

    #region Initiatives
    internal DbSet<Initiative> Initiatives { get; set; }
    #endregion

    #region Items
    internal DbSet<Item> Items { get; set; }
    internal DbSet<ItemField> ItemFields { get; set; }
    internal DbSet<ItemFieldListEntry> ItemFieldListEntries { get; set; }
    internal DbSet<ItemValue> ItemValues { get; set; }
    internal DbSet<ItemVersion> ItemVersions { get; set; }
    
    internal DbSet<Tag> Tags { get; set; }
    #endregion

    #region Layouts
    internal DbSet<Layout> Layouts { get; set; }
    internal DbSet<LayoutVersion> LayoutVersions { get; set; }
    #endregion

    #region Projects
    internal DbSet<Project> Projects { get; set; }
    #endregion

    #region Queues
    internal DbSet<Queue> Queues { get; set; }
    #endregion
    
    
    #endregion


    /// <summary>
    ///     Configure which database to use: PostgreSQL in most cases, in-memory DB for unit tests.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <exception cref="UnrecoverableException"></exception>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string useInMemoryDbStr = configuration["Database:UseInMemoryDb"] ?? "false";
        bool useInMemoryDb = bool.Parse(useInMemoryDbStr);

        if (useInMemoryDb)
        {
            // Name needs to be unique, else the tests will clobber each other
            optionsBuilder.UseInMemoryDatabase($"BonesInMemoryDb-{Guid.NewGuid()}",
                options => { options.EnableNullChecks(); });
        }
        else
        {
            string? connectionString = configuration.GetConnectionString(BonesDbConnectionStringKey);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new BonesException("Missing appsettings configuration for connection string: BonesDb");
            }

            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    /// <summary>
    ///     Create models
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        const string identitySchema = "Identity";
        builder.Entity<BonesUser>().ToTable("BonesUsers", identitySchema);
        builder.Entity<BonesUserRole>().ToTable("BonesUserRoles", identitySchema);
        builder.Entity<BonesUserLogin>().ToTable("BonesUserLogins", identitySchema);
        builder.Entity<BonesUserClaim>().ToTable("BonesUserClaims", identitySchema);
        builder.Entity<BonesUserToken>().ToTable("BonesUserTokens", identitySchema);
        builder.Entity<BonesRole>().ToTable("BonesRoles", identitySchema);
        builder.Entity<BonesRoleClaim>().ToTable("BonesRoleClaims", identitySchema);
    }
}