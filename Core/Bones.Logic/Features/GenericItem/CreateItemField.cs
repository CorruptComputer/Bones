using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects;
using Bones.Shared.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <inheritdoc />
public sealed class CreateItemField(ISender sender) : IRequestHandler<CreateItemField.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a new item field
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    /// <param name="Name"></param>
    /// <param name="IsRequired"></param>
    /// <param name="Type"></param>
    /// <param name="CanBeNegative"></param>
    /// <param name="PossibleValues"></param>
    /// <param name="GeoLocationType"></param>
    /// <param name="RequiredAddressFields"></param>
    /// <param name="RequestingUser"></param>
    public record Command(Guid ProjectId, string Name, bool IsRequired, FieldType Type,
        bool? CanBeNegative, Dictionary<string, StringValueMatchingType>? PossibleValues,
        GeoLocationType? GeoLocationType, AddressFields? RequiredAddressFields, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(512);
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.CanBeNegative).NotNull().When(x => x.Type is FieldType.Integer or FieldType.Decimal);
            RuleFor(x => x.PossibleValues).NotEmpty().When(x => x.Type == FieldType.ValueList);
            RuleFor(x => x.GeoLocationType).NotNull().When(x => x.Type == FieldType.GeoLocation);
            RuleFor(x => x.RequiredAddressFields).NotNull().When(x => x.GeoLocationType == GeoLocationType.Address);
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

        CommandResponse createFieldResponse = await sender.Send(new CreateItemFieldDb.Command(request.ProjectId), cancellationToken);
        if (!createFieldResponse.Success || createFieldResponse.Ids.Count == 0)
        {
            return createFieldResponse;
        }

        CommandResponse createFieldVersionResponse = await sender.Send(
            new CreateItemFieldVersionDb.Command(createFieldResponse.Ids[nameof(GenericItemField)], request.Name,
            request.IsRequired, request.Type, request.CanBeNegative, request.PossibleValues,
            request.GeoLocationType, request.RequiredAddressFields), cancellationToken);

        if (!createFieldVersionResponse.Success)
        {
            return createFieldVersionResponse;
        }

        createFieldResponse.Ids[nameof(GenericItemFieldVersion)] = createFieldVersionResponse.Ids[nameof(GenericItemFieldVersion)];

        return createFieldResponse;
    }
}
