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

    internal DbSet<UserEmailVerification> UserEmailVerifications { get; set; }

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




    internal DbSet<Tag> Tags { get; set; }
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
                throw new UnrecoverableException("Missing appsettings configuration for connection string: BonesDb");
            }

            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    /// <summary>
    ///     Create models
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Identity
        modelBuilder.Entity<BonesUser>().ToTable("BonesUsers", "Identity");
        modelBuilder.Entity<BonesUserRole>().ToTable("BonesUserRoles", "Identity");
        modelBuilder.Entity<BonesUserLogin>().ToTable("BonesUserLogins", "Identity");
        modelBuilder.Entity<BonesUserClaim>().ToTable("BonesUserClaims", "Identity");
        modelBuilder.Entity<BonesUserToken>().ToTable("BonesUserTokens", "Identity");
        modelBuilder.Entity<BonesRole>().ToTable("BonesRoles", "Identity");
        modelBuilder.Entity<BonesRoleClaim>().ToTable("BonesRoleClaims", "Identity");
    }
}