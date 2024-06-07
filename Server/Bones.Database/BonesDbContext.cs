using Bones.Database.DbSets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Bones.Database;

internal class BonesDbContext(IConfiguration configuration) : DbContext
{
    private const string BonesDbConnectionStringKey = "BonesDb";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string useInMemoryDbStr = configuration["Database:UseInMemoryDb"] ?? "false";
        bool useInMemoryDb = bool.Parse(useInMemoryDbStr);

        if (useInMemoryDb)
        {
            // Name needs to be unique, else the tests will clobber each other
            optionsBuilder.UseInMemoryDatabase($"BonesInMemoryDb-{Guid.NewGuid()}", options =>
            {
                options.EnableNullChecks();
            });
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

    internal DbSet<Account> Accounts { get; set; }

    internal DbSet<AccountEmailVerification> AccountEmailVerifications { get; set; }

    internal DbSet<TaskSchedule> TaskSchedules { get; set; }

    internal DbSet<TaskHistory> TaskHistories { get; set; }
}