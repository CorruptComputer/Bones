using Bones.Database.DbSets.OrganizationManagement;

namespace Bones.Database.Operations.OrganizationManagement;

/// <inheritdoc />
public sealed class GetOrganizationByIdDb(BonesDbContext dbContext) : IRequestHandler<GetOrganizationByIdDb.Query, QueryResponse<BonesOrganization>>
{
    /// <summary>
    ///   DB Query to get the specified organization.
    /// </summary>
    /// <param name="OrganizationId"></param>
    public sealed record Query(Guid OrganizationId) : IRequest<QueryResponse<BonesOrganization>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.OrganizationId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesOrganization>> Handle(Query request, CancellationToken cancellationToken)
    {
        BonesOrganization? organization = await dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

        if (organization is null)
        {
            return QueryResponse<BonesOrganization>.Fail("Organization not found");
        }

        return organization;
    }
}