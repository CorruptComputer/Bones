using Bones.Database.DbSets;
using Bones.Database.DbSets.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Bones.Database;

/// <summary>
///     Database context for the application.
/// </summary>
/// <param name="configuration">App configuration</param>
public class BonesDbContext(IConfiguration configuration)
    : IdentityDbContext<Account, Group, Guid, AccountPermission, AccountGroup, AccountLogin, GroupPermission,
        AccountToken>
{
    private const string BonesDbConnectionStringKey = "BonesDb";

    internal DbSet<AccountEmailVerification> AccountEmailVerifications { get; set; }

    internal DbSet<TaskSchedule> TaskSchedules { get; set; }

    internal DbSet<TaskHistory> TaskHistories { get; set; }

    /// <summary>
    ///     Configure which database to use: PostgreSQL in most cases, in-memory DB for unit tests.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <exception cref="BonesDatabaseException"></exception>
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
                throw new BonesDatabaseException("Missing appsettings configuration for connection string: BonesDb");
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
        modelBuilder.Entity<Account>().ToTable("Accounts", "Identity");
        modelBuilder.Entity<AccountGroup>().ToTable("AccountGroups", "Identity");
        modelBuilder.Entity<AccountLogin>().ToTable("AccountLogins", "Identity");
        modelBuilder.Entity<AccountPermission>().ToTable("AccountPermissions", "Identity");
        modelBuilder.Entity<AccountToken>().ToTable("AccountTokens", "Identity");
        modelBuilder.Entity<Group>().ToTable("Groups", "Identity");
        modelBuilder.Entity<GroupPermission>().ToTable("GroupPermissions", "Identity");
    }

    #region Identity

    internal DbSet<Account> Accounts { get; set; }

    internal DbSet<AccountGroup> AccountGroups { get; set; }

    internal DbSet<AccountLogin> AccountLogins { get; set; }

    internal DbSet<AccountPermission> AccountPermissions { get; set; }

    internal DbSet<AccountToken> AccountTokens { get; set; }

    internal DbSet<Group> Groups { get; set; }

    internal DbSet<GroupPermission> GroupPermissions { get; set; }

    #endregion
}