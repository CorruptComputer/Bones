using Bones.Logic.Features.Projects;
using Bones.Database.DbSets.Accounts;
using Bones.Api.Models.Project;
using Bones.Api.Controllers.Base;
using Bones.Shared.Backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Projects
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class ProjectSearchController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets the projects for the current user, or specified organization
    /// </summary>
    /// <param name="ownerType">The type of owner, User if self</param>
    /// <param name="organizationId">The ID of the organization, if applicable</param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("by-owner", Name = "SearchProjectsByOwnerAsync")]
    [ProducesResponseType<Dictionary<Guid, string>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Dictionary<Guid, string>>> GetProjectsByOwnerAsync([FromQuery][Required] OwnershipType ownerType, [FromQuery] Guid? organizationId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Dictionary<Guid, string>> response = await Sender.Send(new GetProjectsByOwner.Query(ownerType, organizationId ?? currentUser.Id, currentUser));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        if (response.Result == null || response.Result.Count == 0)
        {
            return NoContent();
        }

        return response.Result;
    }

    /// <summary>
    ///   Gets the users current quick select projects
    /// </summary>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("quick-select", Name = "GetProjectQuickSelectAsync")]
    [ProducesResponseType<List<GetProjectQuickSelectResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<IEnumerable<GetProjectQuickSelectResponse>>> GetProjectQuickSelectAsync()
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();

        // TODO: Make this customizable
        QueryResponse<Dictionary<Guid, string>> response = await Sender.Send(new GetProjectsUserCanAccess.Query(currentUser));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        if (response.Result is null)
        {
            return NotFound(EmptyResponse.Value);
        }

        return response.Result.Select(kvp =>
            new GetProjectQuickSelectResponse
            {
                ProjectId = kvp.Key,
                ProjectName = kvp.Value
            }).ToList();
    }
    #endregion
}