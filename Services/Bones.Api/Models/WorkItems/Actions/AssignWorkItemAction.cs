using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to assign a work item to a user.
/// </summary>
public class AssignWorkItemAction : WorkItemActionBase
{
    internal override void ToInternal(BonesUser user)
    {
        throw new NotImplementedException();
    }
}
