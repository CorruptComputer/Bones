using Questy;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Shared.Backend.Models;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Database.DbSets.ProjectManagement;

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
    protected async Task<BonesUser> GetBackgroundServiceUserAsync()
    {
        BonesUser? user = await Sender.Send(new GetBackgroundServiceUserDb.Query());

        if (user is null)
        {
            throw new InvalidDataException("Background service user not found");
        }

        return user;
    }

    /// <summary>
    ///   Creates an empty project to use for the test
    /// </summary>
    /// <param name="projectName"></param>
    /// <returns></returns>
    protected async Task<Guid> CreateEmptyProject(string projectName)
    {
        CommandResponse response = await Sender.Send(new CreateProjectDb.Command(projectName, await GetBackgroundServiceUserAsync(), null));

        if (!response.Success || response.Ids.Count == 0)
        {
            throw new InvalidDataException("Project creation failed or an ID wasn't returned");
        }

        return response.Ids[nameof(Project)];
    }

    // TODO: Eventually there should be some that setup items and whatnot, but this'll do for now
}