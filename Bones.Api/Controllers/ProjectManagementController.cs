using Bones.Database.DbSets.Identity;

namespace Bones.Api.Controllers;

public partial class ProjectManagementController(BonesUser user, ISender sender) : BonesControllerBase(sender)
{
    private readonly BonesUser _user = user;
}