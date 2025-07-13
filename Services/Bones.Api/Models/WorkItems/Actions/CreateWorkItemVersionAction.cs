using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to create a new version of a work item.
/// </summary>
public class CreateWorkItemVersionAction : WorkItemActionBase
{
    internal override void ToInternal(BonesUser user)
    {
        throw new NotImplementedException();
    }
}
