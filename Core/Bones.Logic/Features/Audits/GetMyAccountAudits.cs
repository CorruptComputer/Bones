using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.Operations.Audit;

namespace Bones.Logic.Features.Audits;

/// <inheritdoc />
public class GetMyAccountAudits(ISender sender) : IRequestHandler<GetMyAccountAudits.Query, QueryResponse<List<AccountAudit>>>
{
    /// <summary>
    ///     
    /// </summary>
    /// <param name="RequestingUser"></param>
    public record Query(BonesUser RequestingUser) : IRequest<QueryResponse<List<AccountAudit>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<AccountAudit>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetAccountAuditsByAccountIdDb.Query(request.RequestingUser.Id), cancellationToken);
    }
}
