using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.Operations.Item;
using Bones.Logic.Features.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Item;

/// <inheritdoc />
public class CreateItemLayout(ISender sender) : IRequestHandler<CreateItemLayout.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an Item Layout
    /// </summary>
    /// <param name="ProjectId"></param>
    /// <param name="Name"></param>
    /// <param name="LayoutUse"></param>
    /// <param name="FriendlyIdPrefix"></param>
    /// <param name="FieldVersions"></param>
    /// <param name="AssigneeDefinitions"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid ProjectId, string Name, ItemLayoutUse LayoutUse, string FriendlyIdPrefix, Dictionary<int, Guid> FieldVersions, Dictionary<int, (string name, AssignmentType assType, SelectionType selType)> AssigneeDefinitions, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.LayoutUse).NotNull().NotEqual(ItemLayoutUse.None);
            RuleFor(x => x.FriendlyIdPrefix).NotEmpty().MaximumLength(6).Matches(@"^[a-zA-Z]*$");
            RuleFor(x => x.FieldVersions).NotEmpty();
            RuleFor(x => x.AssigneeDefinitions).NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.EDIT_PROJECT_SETTINGS;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(request.ProjectId, request.RequestingUser, perm), cancellationToken);

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

            if (field.Project.Id != request.ProjectId)
            {
                return CommandResponse.Forbid();
            }
        }

        ItemLayout? existingLayout = await sender.Send(new GetItemLayoutByProjectAndFriendlyIdPrefixDb.Query(request.ProjectId, request.FriendlyIdPrefix), cancellationToken);
        if (existingLayout != null)
        {
            return CommandResponse.Fail("FriendlyIdPrefix already in use");
        }

        CommandResponse createLayoutResponse = await sender.Send(new CreateItemLayoutDb.Command(request.ProjectId, request.FriendlyIdPrefix), cancellationToken);

        if (!createLayoutResponse.Success || createLayoutResponse.Ids.Count == 0)
        {
            return createLayoutResponse;
        }

        CommandResponse createLayoutVersionResponse = await sender.Send(new CreateItemLayoutVersionDb.Command(createLayoutResponse.Ids[nameof(ItemLayout)], request.Name, request.LayoutUse, request.FieldVersions, request.AssigneeDefinitions), cancellationToken);

        if (!createLayoutVersionResponse.Success)
        {
            return createLayoutVersionResponse;
        }

        createLayoutResponse.Ids[nameof(ItemLayoutVersion)] = createLayoutVersionResponse.Ids[nameof(ItemLayoutVersion)];

        return createLayoutResponse;
    }
}