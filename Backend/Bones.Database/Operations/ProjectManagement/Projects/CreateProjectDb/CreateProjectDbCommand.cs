using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

/// <summary>
///     DB Command for creating a Project.
/// </summary>
/// <param name="Name">Name of the project</param>
public record CreateProjectDbCommand(string Name, BonesUser RequestingUser) : IValidatableRequest<CommandResponse>
{
    /// <inheritdoc />
    public (bool valid, string? invalidReason) IsRequestValid()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return (false, "");
        }

        return (true, null);
    }
}