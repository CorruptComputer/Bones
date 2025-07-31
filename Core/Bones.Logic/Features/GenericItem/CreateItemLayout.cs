using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <inheritdoc />
public class CreateItemLayout(ISender sender) : IRequestHandler<CreateItemLayout.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an Item Layout
    /// </summary>
    /// <param name="ProjectId"></param>
    /// <param name="Name"></param>
    /// <param name="EnabledFor"></param>
    /// <param name="FriendlyIdPrefix"></param>
    /// <param name="FieldVersions"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid ProjectId, string Name, ItemLayoutUses EnabledFor, string FriendlyIdPrefix, Dictionary<uint, Guid> FieldVersions, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.EnabledFor).NotNull().NotEqual(ItemLayoutUses.None);
            RuleFor(x => x.FriendlyIdPrefix).NotNull().NotEmpty().MaximumLength(6).Matches(@"^[a-zA-Z]*$");
            RuleFor(x => x.FieldVersions).NotNull().NotEmpty();
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
            GenericItemFieldVersion? fieldVersion = await sender.Send(new GetItemFieldVersionByIdDb.Query(fieldVersionId), cancellationToken);
            if (fieldVersion == null)
            {
                return CommandResponse.Fail("FieldVersion not found");
            }

            GenericItemField? field = await sender.Send(new GetItemFieldByIdDb.Query(fieldVersion.GenericItemField.Id), cancellationToken);

            if (field == null)
            {
                return CommandResponse.Fail("Field not found");
            }

            if (field.Project.Id != request.ProjectId)
            {
                return CommandResponse.Forbid();
            }
        }

        GenericItemLayout? existingLayout = await sender.Send(new GetItemLayoutByProjectAndFriendlyIdPrefixDb.Query(request.ProjectId, request.FriendlyIdPrefix), cancellationToken);
        if (existingLayout != null)
        {
            return CommandResponse.Fail("FriendlyIdPrefix already in use");
        }

        CommandResponse createLayoutResponse = await sender.Send(new CreateItemLayoutDb.Command(request.ProjectId, request.FriendlyIdPrefix), cancellationToken);

        if (!createLayoutResponse.Success || createLayoutResponse.Ids.Count == 0)
        {
            return createLayoutResponse;
        }

        CommandResponse createLayoutVersionResponse = await sender.Send(new CreateItemLayoutVersionDb.Command(createLayoutResponse.Ids[nameof(GenericItemLayout)], request.Name, request.EnabledFor, request.FieldVersions), cancellationToken);

        if (!createLayoutVersionResponse.Success)
        {
            return createLayoutVersionResponse;
        }

        return createLayoutVersionResponse;
    }
}