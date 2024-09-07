using Bones.Database.DbSets.OrganizationManagement;

namespace Bones.Database.Operations.OrganizationManagement.GetOrganizationByIdDb;

internal sealed class GetOrganizationByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<GetOrganizationByIdDbQuery, QueryResponse<BonesOrganization>>
{
    public async Task<QueryResponse<BonesOrganization>> Handle(GetOrganizationByIdDbQuery request, CancellationToken cancellationToken)
    {
        BonesOrganization? organization = await dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

        if (organization is null)
        {
            return QueryResponse<BonesOrganization>.Fail("Organization not found");
        }

        return organization;
    }
}