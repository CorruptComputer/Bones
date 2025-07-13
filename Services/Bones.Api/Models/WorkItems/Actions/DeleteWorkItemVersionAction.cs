using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to delete a specific version of a work item.
/// </summary>
public class DeleteWorkItemVersionAction : WorkItemActionBase
{
    internal override void ToInternal(BonesUser user)
    {
        throw new NotImplementedException();
    }
}
