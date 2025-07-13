using Bones.Api.Controllers.Base;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Mapping
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class MappingController(ISender sender) : AuthenticatedControllerBase(sender)
{

}
