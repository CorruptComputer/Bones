using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.Operations.Item;
using Bones.Logic.Features.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Item;

/// <inheritdoc />
public class CreateItemLayoutVersion(ISender sender) : IRequestHandler<CreateItemLayoutVersion.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an Item Layout Version
    /// </summary>
    /// <param name="ItemLayoutId"></param>
    /// <param name="Name"></param>
    /// <param name="LayoutUse"></param>
    /// <param name="FieldVersions"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid ItemLayoutId, string Name, ItemLayoutUse LayoutUse, Dictionary<int, Guid> FieldVersions, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.LayoutUse).NotNull().NotEqual(ItemLayoutUse.None);
            RuleFor(x => x.FieldVersions).NotNull().NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        ItemLayout? layout = await sender.Send(new GetItemLayoutByIdDb.Query(request.ItemLayoutId), cancellationToken);

        if (layout == null)
        {
            return CommandResponse.Fail("ItemLayout not found");
        }

        const string perm = BonesClaimTypes.Role.Project.EDIT_PROJECT_SETTINGS;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(layout.Project.Id, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return CommandResponse.Forbid();
        }

        foreach (Guid fieldVersionId in request.FieldVersions.Values)
        {
            ItemFieldVersion? fieldVersion = await sender.Send(new GetItemFieldVersionByIdDb.Query(fieldVersionId), cancellationToken);
            if (fieldVersion == null)
            {
                return CommandResponse.Fail("FieldVersion not found");
            }

            ItemField? field = await sender.Send(new GetItemFieldByIdDb.Query(fieldVersion.ItemField.Id), cancellationToken);

            if (field == null)
            {
                return CommandResponse.Fail("Field not found");
            }

            if (field.Project.Id != layout.Project.Id)
            {
                return CommandResponse.Forbid();
            }
        }

        CommandResponse createLayoutVersionResponse = await sender.Send(new CreateItemLayoutVersionDb.Command(layout.Id, request.Name, request.LayoutUse, request.FieldVersions), cancellationToken);

        if (!createLayoutVersionResponse.Success)
        {
            return createLayoutVersionResponse;
        }

        return createLayoutVersionResponse;
    }
}