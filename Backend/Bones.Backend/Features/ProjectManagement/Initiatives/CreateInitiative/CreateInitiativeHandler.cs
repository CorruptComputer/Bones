using Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives.CreateInitiativeDb;

namespace Bones.Backend.Features.ProjectManagement.Initiatives.CreateInitiative;

internal sealed class CreateInitiativeHandler(ISender sender) : IRequestHandler<CreateInitiativeCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateInitiativeCommand request, CancellationToken cancellationToken)
    {
        // TODO: Validate user permission here
        BonesUser? user = await sender.Send(new GetUserByClaimsPrincipalRequest(request.RequestingUser), cancellationToken);
        
        if (user is null)
        {
            return CommandResponse.Fail("User could not be found");    
        }

        return await sender.Send(new CreateInitiativeDbCommand(request.Name, request.ProjectId), cancellationToken);
    }
}