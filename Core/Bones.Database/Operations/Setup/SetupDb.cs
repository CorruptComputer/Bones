using Bones.Database.Models;

namespace Bones.Database.Operations.Setup;

/// <inheritdoc />
public sealed class SetupDb(BonesDbContext dbContext, DatabaseConfiguration config, ISender sender) : IRequestHandler<SetupDb.Command, CommandResponse>
{
    /// <summary>
    ///   Command to set up the Database
    /// </summary>
    public sealed record Command : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        if (!(config.UseInMemoryDb ?? false) && (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
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

        await sender.Send(new SetupSystemAdminUserAndRole.Command(), cancellationToken);

        return CommandResponse.Pass();
    }
}