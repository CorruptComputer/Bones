using System.Text.Json.Serialization;

namespace Bones.Api.Controllers;

public sealed partial class AccountController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="DisplayName"></param>
    [Serializable]
    [JsonSerializable(typeof(GetMyBasicInfoResponse))]
    public record GetMyBasicInfoResponse(string Email, string DisplayName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="EmailConfirmed"></param>
    /// <param name="EmailConfirmedDateTime"></param>
    /// <param name="DisplayName"></param>
    /// <param name="CreateDateTime"></param>
    [Serializable]
    [JsonSerializable(typeof(GetMyProfileResponse))]
    public record GetMyProfileResponse(string Email, bool EmailConfirmed, DateTimeOffset? EmailConfirmedDateTime, string DisplayName, DateTimeOffset CreateDateTime);
}