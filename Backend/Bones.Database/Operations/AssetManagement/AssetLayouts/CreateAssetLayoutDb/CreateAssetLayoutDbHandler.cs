using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement.AssetLayouts;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.AssetManagement.AssetLayouts.CreateAssetLayoutDb;

internal class CreateAssetLayoutDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateAssetLayoutDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateAssetLayoutDbCommand request, CancellationToken cancellationToken)
    {
        AssetLayout? newLayout = null;

        if (request.OwnershipType == OwnershipType.User)
        {
            BonesUser? user = await dbContext.Users.FindAsync([request.OwnerId], cancellationToken);

            if (user == null)
            {
                return CommandResponse.Fail("User does not exist");
            }

            newLayout = new()
            {
                Name = request.Name,
                OwnerType = OwnershipType.User,
                OwningUser = user
            };
        }
        else if (request.OwnershipType == OwnershipType.Organization)
        {
            BonesOrganization? organization = await dbContext.Organizations.FindAsync([request.OwnerId], cancellationToken);
            if (organization == null)
            {
                return CommandResponse.Fail("Organization does not exist");
            }

            newLayout = new()
            {
                Name = request.Name,
                OwnerType = OwnershipType.Organization,
                OwningOrganization = organization
            };
        }

        if (newLayout == null)
        {
            return CommandResponse.Fail("Something went wrong");
        }

        EntityEntry<AssetLayout> added = dbContext.AssetLayouts.Add(newLayout);
        // Go ahead and just create a default version 0 with no fields added
        added.Entity.Versions.Add(new()
        {
            AssetLayout = added.Entity,
            Fields = [],
            Version = 0
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(added.Entity.Id);
    }
}