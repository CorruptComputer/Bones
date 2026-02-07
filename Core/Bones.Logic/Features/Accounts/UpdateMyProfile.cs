using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Audits;
using Bones.Database.Operations.Accounts;
using Bones.Database.Operations.Audits;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public class UpdateMyProfile(ISender sender) : IRequestHandler<UpdateMyProfile.Command, CommandResponse>
{
    /// <summary>
    ///   Request to update the profile of a user
    /// </summary>
    /// <param name="DisplayName">The display name to set for the user</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public sealed record Command(string DisplayName, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.DisplayName).NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        CommandResponse resp = await sender.Send(new UpdateProfileDb.Command(request.RequestingUser.Id, request.DisplayName), cancellationToken);
        await sender.Send(new AddAccountAuditDb.Command(request.RequestingUser, AccountAudit.Actions.UpdateProfile, request.RequestingUser, AuditActionReasons.UserRequested), cancellationToken);

        return resp;
    }
}