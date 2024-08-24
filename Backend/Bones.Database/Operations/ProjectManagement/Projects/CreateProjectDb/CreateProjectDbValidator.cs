using FluentValidation;

namespace Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

public class CreateProjectDbValidator : AbstractValidator<CreateProjectDbCommand>
{
    public CreateProjectDbValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Project name is required.");
    }
}