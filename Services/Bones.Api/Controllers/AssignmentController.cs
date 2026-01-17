using Bones.Api.Controllers.Base;
using Bones.Api.Models.Assignment;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Logic.Features.Assets;
using Bones.Logic.Features.Tasks.Tasks;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Assets
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class AssignmentController(ISender sender) : AuthenticatedControllerBase(sender)
{

}
