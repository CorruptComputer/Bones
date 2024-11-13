using System.Text.Json.Serialization;

namespace Bones.Api.Controllers;

public sealed partial class AccountController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="EmailConfirmed"></param>
    /// <param name="EmailConfirmedDateTime"></param>
    /// <param name="DisplayName"></param>
    /// <param name="CreateDateTime"></param>
    /// <param name="IsSysAdmin"></param>
    [Serializable]
    [JsonSerializable(typeof(GetMyProfileResponse))]
    public record GetMyProfileResponse(string Email, bool EmailConfirmed, DateTimeOffset? EmailConfirmedDateTime, string DisplayName, DateTimeOffset CreateDateTime, bool IsSysAdmin);
}