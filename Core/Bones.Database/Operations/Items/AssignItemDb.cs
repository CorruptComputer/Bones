using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.Items;

/// <inheritdoc />
public sealed class AssignItemDb(BonesDbContext dbContext) : IRequestHandler<AssignItemDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for assigning an item to a user or role
    /// </summary>
    /// <param name="ItemId">Internal ID of the item</param>
    /// <param name="AssignmentSlotId">ID of the assignment slot slot</param>
    /// <param name="UserId">ID of the user to assign</param>
    /// <param name="State">State for the assignment</param>
    public sealed record Command(Guid ItemId, Guid AssignmentSlotId, Guid UserId, string State) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.AssignmentSlotId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.State).NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Item? item = await dbContext.Items
            .Include(i => i.Versions)
                .ThenInclude(iv => iv.Assignees)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

        if (item?.Current == null)
        {
            return CommandResponse.Fail("Invalid Item ID.");
        }

        ItemAssignmentSlot? assigneeSlot = item.Current.ItemLayoutVersion.AssigneeSlots.FirstOrDefault(x => x.Id == request.AssignmentSlotId);
        if (assigneeSlot == null)
        {
            return CommandResponse.Fail("Invalid Assignment Slot ID.");
        }

        BonesUser? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            return CommandResponse.Fail("Invalid User ID.");
        }

        ItemAssignee newAssignee = new()
        {
            Slot = assigneeSlot,
            AssignedUser = user,
            AssignedRole = null,
            State = request.State
        };

        item.Current.Assignees.Add(newAssignee);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ItemAssignee), newAssignee.Id);
    }
}
