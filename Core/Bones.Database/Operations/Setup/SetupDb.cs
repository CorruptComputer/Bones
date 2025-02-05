using Bones.Database.Models;

namespace Bones.Database.Operations.Setup;

/// <summary>
///   Command to set up the Database
/// </summary>
public sealed record SetupDbCommand : IRequest<CommandResponse>;

internal sealed class SetupDbHandler(BonesDbContext dbContext, DatabaseConfiguration config, ISender sender) : IRequestHandler<SetupDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(SetupDbCommand request, CancellationToken cancellationToken)
    {
        if (!(config.UseInMemoryDb ?? false) && (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            Log.Information("Migration complete.");
        }
        else
        {
            Log.Information("Database is up to date.");
        }

        await sender.Send(new SetupSystemAdminUserAndRoleCommand(), cancellationToken);

        return CommandResponse.Pass();
    }
}