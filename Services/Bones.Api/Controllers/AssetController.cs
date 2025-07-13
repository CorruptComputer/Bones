using Bones.Api.Controllers.Base;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Assets
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class AssetController(ISender sender) : AuthenticatedControllerBase(sender)
{

}
