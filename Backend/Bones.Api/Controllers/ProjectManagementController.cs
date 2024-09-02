namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Projects
/// </summary>
/// <param name="sender">MediatR sender</param>
public sealed partial class ProjectManagementController(ISender sender) : BonesControllerBase(sender);