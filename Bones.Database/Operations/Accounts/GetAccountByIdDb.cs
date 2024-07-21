using Bones.Database.DbSets.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///     DB Query to get an Account by AccountId
/// </summary>
/// <param name="AccountId">ID of the account to get</param>
public record GetAccountByIdDbQuery(Guid AccountId) : IRequest<QueryResponse<Account>>;

internal class GetAccountByIdDbHandler(BonesDbContext dbContext)
    : IRequestHandler<GetAccountByIdDbQuery, QueryResponse<Account>>
{
    public async Task<QueryResponse<Account>> Handle(GetAccountByIdDbQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);
    }
}