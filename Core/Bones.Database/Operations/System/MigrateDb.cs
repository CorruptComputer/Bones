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
        if (!config.UseInMemoryDb && (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            // TODO: Remove this at some point, just easier to do this while still in development and major DB changes are still happening
            await dbContext.Database.EnsureDeletedAsync(cancellationToken);

            await dbContext.Database.MigrateAsync(cancellationToken);
            Log.Information("Migration complete.");
        }
        else
        {
            Log.Information("Database is up to date.");
        }

        return CommandResponse.Pass();
    }
}