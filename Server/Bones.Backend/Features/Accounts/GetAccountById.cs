using Bones.Backend.Models;
using Bones.Database.DbSets;
using Bones.Database.Models;
using Bones.Database.Operations.Accounts;
using MediatR;

namespace Bones.Backend.Features.Accounts;

/// <summary>
///   Query for getting an Account's details by the Account ID.
/// </summary>
/// <param name="AccountId">Account ID that you want to get the details of.</param>
public record GetAccountByIdQuery(long AccountId) : IRequest<QueryResponse<GetAccountByIdResponse>>;

/// <summary>
///   Response for getting an Account's details by the Account ID.
/// </summary>
/// <param name="Email">Account's email address.</param>
/// <param name="CreateDateTime">Account's create date.</param>
public record GetAccountByIdResponse(string Email, DateTimeOffset CreateDateTime);

internal class GetAccountByIdHandler(ISender sender) : IRequestHandler<GetAccountByIdQuery, QueryResponse<GetAccountByIdResponse>>
{
    public async Task<QueryResponse<GetAccountByIdResponse>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        DbQueryResponse<Account> dbResponse =
            await sender.Send(new GetAccountByIdDbQuery(request.AccountId), cancellationToken);

        if (!dbResponse.Success || dbResponse.Result == null)
        {
            return new()
            {
                Success = false,
                FailureReason = dbResponse.FailureReason ?? "Unable to find account."
            };
        }

        return new()
        {
            Success = true,
            Result = new(dbResponse.Result.Email, dbResponse.Result.CreateDateTime)
        };
    }
}