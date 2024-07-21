using Bones.Database.DbSets.Identity;
using Bones.Database.Operations.Accounts;

namespace Bones.Api.Features.Accounts;

/// <summary>
///     Query for getting an Account's details by the Account ID.
/// </summary>
/// <param name="AccountId">Account ID that you want to get the details of.</param>
public record GetAccountByIdQuery(Guid AccountId) : IRequest<QueryResponse<GetAccountByIdResponse>>;

/// <summary>
///     Response for getting an Account's details by the Account ID.
/// </summary>
/// <param name="Email">Account's email address.</param>
/// <param name="CreateDateTime">Account's create date.</param>
public record GetAccountByIdResponse(string Email, DateTimeOffset CreateDateTime);

internal class GetAccountByIdHandler(ISender sender)
    : IRequestHandler<GetAccountByIdQuery, QueryResponse<GetAccountByIdResponse>>
{
    public async Task<QueryResponse<GetAccountByIdResponse>> Handle(GetAccountByIdQuery request,
        CancellationToken cancellationToken)
    {
        QueryResponse<Account> response =
            await sender.Send(new GetAccountByIdDbQuery(request.AccountId), cancellationToken);

        if (!response.Success || response.Result?.Email == null)
        {
            return new()
            {
                Success = false,
                FailureReason = response.FailureReason ?? "Unable to find account."
            };
        }

        return new()
        {
            Success = true,
            Result = new(response.Result.Email, response.Result.CreateDateTime)
        };
    }
}