using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to move a work item to a different queue.
/// </summary>
public class MoveWorkItemQueueAction : WorkItemActionBase
{
    internal override void ToInternal(BonesUser user)
    {
        throw new NotImplementedException();
    }
}
