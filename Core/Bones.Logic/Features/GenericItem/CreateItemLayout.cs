using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <summary>
///   DB Command for creating an Item Layout
/// </summary>
/// <param name="ProjectId"></param>
/// <param name="Name"></param>
/// <param name="EnabledFor"></param>
/// <param name="FriendlyIdPrefix"></param>
/// <param name="FieldVersions"></param>
/// <param name="RequestingUser"></param>
public sealed record CreateItemLayoutCommand(Guid ProjectId, string Name, ItemLayoutUses EnabledFor, string FriendlyIdPrefix, List<Guid> FieldVersions, BonesUser RequestingUser) : IRequest<CommandResponse>;

internal class CreateItemLayoutCommandValidator : AbstractValidator<CreateItemLayoutCommand>
{
    public CreateItemLayoutCommandValidator() 
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.EnabledFor).NotNull().NotEqual(ItemLayoutUses.None);
        RuleFor(x => x.FriendlyIdPrefix).NotNull().NotEmpty().MaximumLength(6).Matches(@"^[a-zA-Z]*$");
        RuleFor(x => x.FieldVersions).NotNull().NotEmpty();
        RuleFor(x => x.RequestingUser).NotNull();
    }
}

internal class CreateItemLayoutHandler(ISender sender) : IRequestHandler<CreateItemLayoutCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateItemLayoutCommand request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.EDIT_PROJECT_SETTINGS;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermissionQuery(request.ProjectId, request.RequestingUser, perm), cancellationToken);

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

            if (field.ProjectId != request.ProjectId)
            {
                return CommandResponse.Forbid();
            }
        }

        GenericItemLayout? existingLayout = await sender.Send(new GetItemLayoutByProjectAndFriendlyIdPrefixDb.Query(request.ProjectId, request.FriendlyIdPrefix), cancellationToken);
        if (existingLayout != null)
        {
            return CommandResponse.Fail("FriendlyIdPrefix already in use");
        }

        CommandResponse createLayoutResponse = await sender.Send(new CreateItemLayoutDbCommand(request.ProjectId, request.FriendlyIdPrefix), cancellationToken);

        if (!createLayoutResponse.Success || createLayoutResponse.Id == null)
        {
            return createLayoutResponse;
        }

        CommandResponse createLayoutVersionResponse = await sender.Send(new CreateItemLayoutVersionDbCommand(createLayoutResponse.Id.Value, request.Name, request.EnabledFor, request.FieldVersions), cancellationToken);

        if (!createLayoutVersionResponse.Success)
        {
            return createLayoutVersionResponse;
        }

        return createLayoutVersionResponse;
    }
}