using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Controllers;

public sealed partial class ProjectController
{
    /// <summary>
    ///   Request to create a new project
    /// </summary>
    /// <param name="Name">Name of the project to create</param>
    /// <param name="OrganizationId">Optionally the organization that this should be created under, if not specified will be created for the requesting user.</param>
    [Serializable]
    [JsonSerializable(typeof(CreateProjectRequest))]
    public record CreateProjectRequest([Required] string Name, Guid? OrganizationId = null);

    /// <summary>
    ///   Request to create a new initiative
    /// </summary>
    /// <param name="Name">Name of the initiative to create</param>
    [Serializable]
    [JsonSerializable(typeof(CreateInitiativeRequest))]
    public record CreateInitiativeRequest([Required] string Name);

    /// <summary>
    ///   Request to get the projects for a given User/Organization
    /// </summary>
    /// <param name="OwnerType">OwnerType to get</param>
    /// <param name="OrganizationId">Optionally the organization that this should be created under, if not specified will be created for the requesting user.</param>
    [Serializable]
    [JsonSerializable(typeof(GetProjectsByOwnerRequest))]
    public record GetProjectsByOwnerRequest([Required] OwnershipType OwnerType, Guid? OrganizationId = null);
}