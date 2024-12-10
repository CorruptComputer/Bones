using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Controllers;

public sealed partial class ProjectController
{
    /// <summary>
    ///   Request to create a new project
    /// </summary>
    [JsonSerializable(typeof(CreateProjectRequest))]
    public record CreateProjectRequest
    {
        /// <summary>
        ///   Name of the project to create
        /// </summary>
        [Required]
        public required string Name { get; init; }

        /// <summary>
        ///   Optionally the organization that this should be created under, if not specified will be created for the requesting user.
        /// </summary>
        public Guid? OrganizationId { get; init; }
    }

    /// <summary>
    ///   Request to create a new initiative
    /// </summary>
    [JsonSerializable(typeof(CreateInitiativeRequest))]
    public record CreateInitiativeRequest
    {
        /// <summary>
        ///   Name of the initiative to create
        /// </summary>
        [Required]
        public required string Name { get; init; }
    }

    /// <summary>
    ///   Request to get the projects for a given User/Organization
    /// </summary>
    [JsonSerializable(typeof(GetProjectsByOwnerRequest))]
    public record GetProjectsByOwnerRequest
    {
        /// <summary>
        ///   OwnerType to get
        /// </summary>
        [Required]
        public required OwnershipType OwnerType { get; init; }

        /// <summary>
        ///   Optionally the organization that this should be created under, if not specified will be created for the requesting user.
        /// </summary>
        public Guid? OrganizationId { get; init; }
    }
}