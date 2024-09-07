namespace Bones.Database.Operations.ProjectManagement.Tags.QueueDeleteTagByIdDb;

internal sealed class DeleteTagByIdDbCommandValidator : AbstractValidator<DeleteTagByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<DeleteTagByIdDbCommand> context, CancellationToken cancellation = new CancellationToken())
    {
        RuleFor(x => x.TagId).NotNull().NotEqual(Guid.Empty);
        
        return base.ValidateAsync(context, cancellation);
    }
}