using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;

namespace Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

/// <summary>
///     DB Command for creating a Project.
/// </summary>
/// <param name="Name">Name of the project</param>
/// <param name="RequestingUser">The user requesting this projects creation</param>
/// <param name="Organization">Optionally, the organization this project should belong to</param>
public sealed record CreateProjectDbCommand(string Name, BonesUser RequestingUser, BonesOrganization? Organization) : IRequest<CommandResponse>;