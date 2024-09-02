using System.Security.Claims;
using Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

namespace Bones.Backend.Features.ProjectManagement.Projects;

public class CreateProject(ISender sender) : IRequestHandler<CreateProject.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a Project.
    /// </summary>
    /// <param name="Name">Name of the project</param>
    public record Command(string Name, ClaimsPrincipal RequestingUser, Guid? OrganizationId = null) : IRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesUser? user = await sender.Send(new GetUserByClaimsPrincipalRequest(request.RequestingUser), cancellationToken);

        if (user == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "User not found"
            };
        }
        
        BonesOrganization? organization = null;
        if (request.OrganizationId.HasValue)
        {
            //organization = sender.Send(new GetOrganizationByIdDB(request.OrganizationId.Value), cancellationToken);
            
            // TODO: Make sure the user has the right to do this in the organization before sending the request off to the DB
        }
        
        return await sender.Send(new CreateProjectDbCommand(request.Name, user, organization), cancellationToken);
    }
}