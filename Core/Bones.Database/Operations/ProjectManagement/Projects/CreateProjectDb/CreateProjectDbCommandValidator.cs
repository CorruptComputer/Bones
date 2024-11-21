namespace Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

internal sealed class CreateProjectDbCommandValidator : AbstractValidator<CreateProjectDbCommand>
{
    public CreateProjectDbCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Project name is required.");
    }
}