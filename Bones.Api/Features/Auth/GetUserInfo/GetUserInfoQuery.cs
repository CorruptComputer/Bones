using System.Security.Claims;

namespace Bones.Api.Features.Auth.GetUserInfo;

public class GetUserInfoQuery : IValidatableRequest<QueryResponse<GetUserInfoResponse>>
{
    
    public required ClaimsPrincipal ClaimsPrincipal { get; init; }
    
    public (bool valid, string? invalidReason) IsRequestValid()
    {
        throw new NotImplementedException();
    }
}