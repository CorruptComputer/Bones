using Questy;
using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Shared.Backend.Models;
using Bones.Database.Operations.Projects.Projects;
using Bones.Database.DbSets.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Logic.Features.Projects;
using Bones.Testing.Shared.Exceptions;

namespace Bones.Testing.UnitTests.Shared;

/// <summary>
///   Base for Unit Test classes.
/// </summary>
public class TestBase
{
    /// <summary>
    ///   Questy sender for commands and queries. Unique DB per-test.
    /// </summary>
    protected ISender Sender { get; } = TestFactory.GetTestSender();

    /// <summary>
    ///   Background service user, will be automatically created on setup. Makes testing things that need a user easier, just use this.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BonesTestException"></exception>
    protected async Task<BonesUser> GetBackgroundServiceUserAsync()
    {
        BonesUser? user = await Sender.Send(new GetBackgroundServiceUserDb.Query());

        BonesTestException.ThrowIfNull(user);

        return user;
    }

    /// <summary>
    ///   Creates an empty project to use for the test
    /// </summary>
    /// <param name="projectName"></param>
    /// <returns></returns>
    /// <exception cref="BonesTestException"></exception>
    protected async Task<Guid> CreateEmptyProject(string projectName)
    {
        CommandResponse response = await Sender.Send(new CreateProjectDb.Command(projectName, await GetBackgroundServiceUserAsync(), null));

        if (!response.Success || response.Ids.Count == 0)
        {
            throw new BonesTestException("Project creation failed or an ID wasn't returned");
        }

        return response.Ids[nameof(Project)];
    }

    /// <summary>
    ///   Creates a project with a preset to use for the test
    /// </summary>
    /// <param name="preset"></param>
    /// <param name="projectName"></param>
    /// <param name="withTasks"></param>
    /// <returns></returns>
    /// <exception cref="BonesTestException"></exception>
    protected async Task<Guid> CreateProjectWithPreset(ProjectPreset preset, string projectName, bool withTasks = false)
    {
        CommandResponse response = await Sender.Send(new CreateProjectWithPreset.Command(projectName, preset, await GetBackgroundServiceUserAsync(), null, withTasks));

        if (!response.Success || response.Ids.Count == 0)
        {
            throw new BonesTestException("Project creation failed or an ID wasn't returned");
        }

        return response.Ids[nameof(Project)];
    }

    // TODO: Eventually there should be some that setup items and whatnot, but this'll do for now
}