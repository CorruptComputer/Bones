using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;

namespace Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

/// <summary>
///     DB Command for creating a Project.
/// </summary>
/// <param name="Name">Name of the project</param>
public sealed record CreateProjectDbCommand(string Name, BonesUser RequestingUser, BonesOrganization? Organization) : IRequest<CommandResponse>;