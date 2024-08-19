namespace Bones.Api.Features.Identity.GetUserInfo;

public class GetUserInfoQuery : IValidatableRequest<QueryResponse<GetUserInfoResponse>>
{
    
    
    public (bool valid, string? invalidReason) IsRequestValid()
    {
        throw new NotImplementedException();
    }
}