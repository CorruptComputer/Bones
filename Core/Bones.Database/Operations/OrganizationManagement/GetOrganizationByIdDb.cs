using Bones.Database.DbSets.OrganizationManagement;

namespace Bones.Database.Operations.OrganizationManagement;

/// <summary>
///   DB Query to get the specified organization.
/// </summary>
/// <param name="OrganizationId"></param>
public sealed record GetOrganizationByIdDbQuery(Guid OrganizationId) : IRequest<QueryResponse<BonesOrganization>>;

internal sealed class GetOrganizationByIdDbQueryValidator : AbstractValidator<GetOrganizationByIdDbQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetOrganizationByIdDbQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.OrganizationId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}

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