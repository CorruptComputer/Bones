using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public class UpdateProfile(ISender sender) : IRequestHandler<UpdateProfile.Command, CommandResponse>
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
            RuleFor(x => x.DisplayName).NotNull().NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new UpdateProfileDb.Command(request.RequestingUser.Id, request.DisplayName), cancellationToken);
    }
}