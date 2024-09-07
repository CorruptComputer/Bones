namespace Bones.Database.Operations.OrganizationManagement.GetOrganizationByIdDb;

internal sealed class GetOrganizationByIdDbQueryValidator : AbstractValidator<GetOrganizationByIdDbQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetOrganizationByIdDbQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.OrganizationId).NotNull().NotEqual(Guid.Empty);
        
        return base.ValidateAsync(context, cancellation);
    }
}