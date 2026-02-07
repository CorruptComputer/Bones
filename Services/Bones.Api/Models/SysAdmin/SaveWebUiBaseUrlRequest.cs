using Bones.Database.DbSets.Accounts;
using Bones.Logic.Features.System.SystemSettings;

namespace Bones.Api.Models.SysAdmin;

/// <summary>
///   Request for the SaveWebUiBaseUrlAsync endpoint
/// </summary>
public class SaveWebUiBaseUrlRequest
{
    /// <summary>
    ///   The base URL for the web UI
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    ///   The reason for the change
    /// </summary>
    public required string ChangeReason { get; init; }

    internal SaveWebUiBaseUrl.Command ToInternal(BonesUser user)
    {
        return new(BaseUrl, ChangeReason, user);
    }
}
