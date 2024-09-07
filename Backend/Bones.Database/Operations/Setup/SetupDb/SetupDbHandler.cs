using Bones.Database.Models;
using Bones.Database.Operations.Setup.SetupSystemAdminUserAndRole;

namespace Bones.Database.Operations.Setup.SetupDb;

internal sealed class SetupDbHandler(BonesDbContext dbContext, DatabaseConfiguration config, ISender sender) : IRequestHandler<SetupDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(SetupDbCommand request, CancellationToken cancellationToken)
    {
        if (!(config.UseInMemoryDb ?? false) && (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        
        CommandResponse admin = await sender.Send(new SetupSystemAdminUserAndRoleCommand(), cancellationToken);

        return CommandResponse.Pass();
    }
}