using Bones.Database.DbSets.Identity;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///     DB Query to get an Account by AccountId
/// </summary>
/// <param name="AccountId">ID of the account to get</param>
public record GetAccountByIdDbQuery(Guid AccountId) : IRequest<DbQueryResponse<Account>>;

internal class GetAccountByIdDbHandler(BonesDbContext dbContext)
    : IRequestHandler<GetAccountByIdDbQuery, DbQueryResponse<Account>>
{
    public async Task<DbQueryResponse<Account>> Handle(GetAccountByIdDbQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);
    }
}