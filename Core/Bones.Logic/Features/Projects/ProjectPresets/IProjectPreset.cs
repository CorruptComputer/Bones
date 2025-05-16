using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Enums;

namespace Bones.Logic.Features.Projects.ProjectPresets;

/// <summary>
///   Interface for a project preset
/// </summary>
public interface IProjectPreset
{
    /// <summary>
    ///   The preset this is, here just for the reference on the enum to make it easy to jump to where its defined
    /// </summary>
    public ProjectPreset Preset { get; }

    /// <summary>
    ///   Creates the item layouts for this preset
    /// </summary>
    /// <returns>Success</returns>
    public Task<bool> CreatePresetLayoutsAsync(ISender sender, Guid projectId, BonesUser requestingUser, CancellationToken cancellationToken);
}
