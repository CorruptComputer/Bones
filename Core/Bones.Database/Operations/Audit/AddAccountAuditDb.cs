using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;

namespace Bones.Database.Operations.Audit;

/// <inheritdoc />
public class AddAccountAuditDb(BonesDbContext dbContext) : IRequestHandler<AddAccountAuditDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating a new account audit entry
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
        AccountAudit audit = new()
        {
            AccountBonesUserId = request.Account.Id,
            ActionDateTime = DateTimeOffset.Now,
            ActionTaken = request.ActionTaken,
            ActionTakenByBonesUserId = request.ActionTakenBy.Id,
            Reason = request.Reason
        };

        dbContext.AccountAudits.Add(audit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
