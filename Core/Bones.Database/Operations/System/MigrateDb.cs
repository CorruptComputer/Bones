namespace Bones.Database.Operations.System;

/// <inheritdoc />
public sealed class MigrateDb(BonesDbContext dbContext, BonesBackendConfiguration config) : IRequestHandler<MigrateDb.Command, CommandResponse>
{
    /// <summary>
    ///   Command to set up the Database
    /// </summary>
    public sealed record Command : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        if (config.UseInMemoryDb)
        {
            Log.Information("Using in-memory database, no migrations needed.");
            
            return CommandResponse.Pass();
        }

        if (config.SetupForTesting)
        {
            Log.Information("Resetting database for testing...");
            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);

            return CommandResponse.Pass();
        }

        IEnumerable<string> pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            Log.Information("Migration complete.");

            return CommandResponse.Pass();
        }

        Log.Information("Database is up to date.");
        return CommandResponse.Pass();
    }
}