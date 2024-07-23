using Bones.Database.DbSets.Identity;
using Bones.Database.Operations.Users;

namespace Bones.Api.Features.Users;

public class GetUserById(ISender sender) : IRequestHandler<GetUserById.Query, QueryResponse<GetUserById.Response>>
{
    /// <summary>
    ///     Query for getting a User's details by the User ID.
    /// </summary>
    /// <param name="UserId">User ID that you want to get the details of.</param>
    public record Query(Guid UserId) : IRequest<QueryResponse<Response>>;

    /// <summary>
    ///     Response for getting a User's details by the User ID.
    /// </summary>
    /// <param name="Email">User's email address.</param>
    /// <param name="CreateDateTime">User's create date.</param>
    public record Response(string Email, DateTimeOffset CreateDateTime);
    
    public async Task<QueryResponse<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        QueryResponse<BonesUser> response =
            await sender.Send(new GetUserByIdDb.Query(request.UserId), cancellationToken);

        if (!response.Success || response.Result?.Email == null)
        {
            return new()
            {
                Success = false,
                FailureReason = response.FailureReason ?? "Unable to find User."
            };
        }

        return new()
        {
            Success = true,
            Result = new(response.Result.Email, response.Result.CreateDateTime)
        };
    }
}