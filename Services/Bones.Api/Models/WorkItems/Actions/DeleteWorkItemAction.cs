using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to delete a work item.
/// </summary>
public class DeleteWorkItemAction : WorkItemActionBase
{
    internal override void ToInternal(BonesUser user)
    {
        throw new NotImplementedException();
    }
}
