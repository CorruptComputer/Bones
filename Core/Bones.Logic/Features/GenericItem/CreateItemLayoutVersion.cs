using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <summary>
///   DB Command for creating an Item Layout Version
/// </summary>
/// <param name="ItemLayoutId"></param>
/// <param name="Name"></param>
/// <param name="EnabledFor"></param>
/// <param name="FieldVersions"></param>
/// <param name="RequestingUser"></param>
public sealed record CreateItemLayoutVersionCommand(Guid ItemLayoutId, string Name, ItemLayoutUses EnabledFor, List<Guid> FieldVersions, BonesUser RequestingUser) : IRequest<CommandResponse>;

internal class CreateItemLayoutVersionCommandValidator : AbstractValidator<CreateItemLayoutVersionCommand>
{
    public CreateItemLayoutVersionCommandValidator() 
    {
        RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.EnabledFor).NotNull().NotEqual(ItemLayoutUses.None);
        RuleFor(x => x.FieldVersions).NotNull().NotEmpty();
        RuleFor(x => x.RequestingUser).NotNull();
    }
}

internal class CreateItemLayoutVersionHandler(ISender sender) : IRequestHandler<CreateItemLayoutVersionCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateItemLayoutVersionCommand request, CancellationToken cancellationToken)
    {
        GenericItemLayout? layout = await sender.Send(new GetItemLayoutByIdDbQuery(request.ItemLayoutId), cancellationToken);

        if (layout == null)
        {
            return CommandResponse.Fail("ItemLayout not found");
        }
        
        const string perm = BonesClaimTypes.Role.Project.EDIT_PROJECT_SETTINGS;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermissionQuery(layout.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return CommandResponse.Forbid();
        }

        foreach (Guid fieldVersionId in request.FieldVersions)
        {
            GenericItemFieldVersion? fieldVersion = await sender.Send(new GetItemFieldVersionByIdDbQuery(fieldVersionId), cancellationToken);
            if (fieldVersion == null)
            {
                return CommandResponse.Fail("FieldVersion not found");
            }

            GenericItemField? field = await sender.Send(new GetItemFieldByIdDbQuery(fieldVersion.GenericItemFieldId), cancellationToken);

            if (field == null)
            {
                return CommandResponse.Fail("Field not found");
            }

            if (field.ProjectId != layout.ProjectId)
            {
                return CommandResponse.Forbid();
            }
        }

        CommandResponse createLayoutVersionResponse = await sender.Send(new CreateItemLayoutVersionDbCommand(layout.Id, request.Name, request.EnabledFor, request.FieldVersions), cancellationToken);

        if (!createLayoutVersionResponse.Success)
        {
            return createLayoutVersionResponse;
        }

        return createLayoutVersionResponse;
    }
}