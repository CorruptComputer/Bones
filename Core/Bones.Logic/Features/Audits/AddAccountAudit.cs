using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.Operations.Audit;

namespace Bones.Logic.Features.Audits;

/// <inheritdoc />
public class AddAccountAudit(ISender sender) : IRequestHandler<AddAccountAudit.Command, CommandResponse>
{
    /// <summary>
    ///     Command for creating a new account audit entry
    /// </summary>
    /// <param name="Account">The account which was acted upon</param>
    /// <param name="ActionTaken"></param>
    /// <param name="ActionTakenBy"></param>
    /// <param name="Reason"></param>
    public record Command(BonesUser Account, AccountAudit.Actions ActionTaken, BonesUser ActionTakenBy, string Reason) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Account).NotNull();
            RuleFor(x => x.ActionTaken).IsInEnum();
            RuleFor(x => x.ActionTakenBy).NotNull();
            RuleFor(x => x.Reason).NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new AddAccountAuditDb.Command(request.Account, request.ActionTaken, request.ActionTakenBy, request.Reason), cancellationToken);
    }
}