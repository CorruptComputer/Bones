using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Database.Operations.GenericItem;
using Bones.Database.Operations.ProjectManagement.Initiatives;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
using Bones.Database.Operations.WorkItemManagement.WorkItems;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.Operations.System.TestingDataSetup.Steps;

/// <inheritdoc />
public class SetupTestProjectDb(ISender sender) : IRequestHandler<SetupTestProjectDb.Command, CommandResponse>
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="OwningUserId"></param>
    public sealed record Command(Guid OwningUserId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesUser? testUser = await sender.Send(new GetUserByEmailDb.Query(DefaultValues.TEST_USER_EMAIL), cancellationToken);
        if (testUser == null)
        {
            return CommandResponse.Fail("No user found. Cannot set up testing data.");
        }

        CommandResponse project = await sender.Send(new CreateProjectDb.Command("Test Project", testUser), cancellationToken);

        if (project.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test project.");
        }

        CommandResponse initiative = await sender.Send(new CreateInitiativeDb.Command("Test Initiative", project.Ids[nameof(Project)]), cancellationToken);
        if (initiative.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test initiative.");
        }

        CommandResponse queue = await sender.Send(new CreateWorkItemQueueDb.Command("Test Queue", initiative.Ids[nameof(Initiative)]), cancellationToken);
        if (queue.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test queue.");
        }

        // Add fields
        CommandResponse field1 = await sender.Send(new CreateItemFieldDb.Command(project.Ids[nameof(Project)]), cancellationToken);
        if (field1.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 1.");
        }

        CommandResponse field2 = await sender.Send(new CreateItemFieldDb.Command(project.Ids[nameof(Project)]), cancellationToken);
        if (field2.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 2.");
        }

        CommandResponse field3 = await sender.Send(new CreateItemFieldDb.Command(project.Ids[nameof(Project)]), cancellationToken);
        if (field3.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 3.");
        }

        CommandResponse field4 = await sender.Send(new CreateItemFieldDb.Command(project.Ids[nameof(Project)]), cancellationToken);
        if (field4.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 4.");
        }

        CommandResponse field5 = await sender.Send(new CreateItemFieldDb.Command(project.Ids[nameof(Project)]), cancellationToken);
        if (field5.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 5.");
        }

        CommandResponse field6 = await sender.Send(new CreateItemFieldDb.Command(project.Ids[nameof(Project)]), cancellationToken);
        if (field6.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 6.");
        }

        CommandResponse field7 = await sender.Send(new CreateItemFieldDb.Command(project.Ids[nameof(Project)]), cancellationToken);
        if (field7.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 7.");
        }

        // Add field versions
        CommandResponse field1v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field1.Ids[nameof(GenericItemField)], "Required Small Text", true, FieldType.TextField, null, null, null, null), cancellationToken);
        if (field1v1.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 1 version 1.");
        }

        CommandResponse field2v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field2.Ids[nameof(GenericItemField)], "Required Integer", true, FieldType.Integer, true, null, null, null), cancellationToken);
        if (field2v1.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 2 version 1.");
        }

        CommandResponse field3v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field3.Ids[nameof(GenericItemField)], "Optional Positive Decimal", false, FieldType.Decimal, false, null, null, null), cancellationToken);
        if (field3v1.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 3 version 1.");
        }

        CommandResponse field4v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field4.Ids[nameof(GenericItemField)], "Required Value List", true, FieldType.ValueList, null, new Dictionary<string, StringValueMatchingType>
        {
            { "Test Value 1", StringValueMatchingType.Exact },
            { "Test Value 2", StringValueMatchingType.CaseInvariant }
        }, null, null), cancellationToken);
        if (field4v1.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 4 version 1.");
        }

        CommandResponse field5v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field5.Ids[nameof(GenericItemField)], "Optional Large Text", false, FieldType.TextBox, null, null, null, null), cancellationToken);
        if (field5v1.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 5 version 1.");
        }

        CommandResponse field6v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field6.Ids[nameof(GenericItemField)], "Optional Boolean", false, FieldType.Boolean, null, null, null, null), cancellationToken);
        if (field6v1.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 6 version 1.");
        }

        CommandResponse field7v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field7.Ids[nameof(GenericItemField)], "Optional DateTime", false, FieldType.DateTime, null, null, null, null), cancellationToken);
        if (field7v1.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test field 7 version 1.");
        }

        // Add a layout to the project
        CommandResponse layout = await sender.Send(new CreateItemLayoutDb.Command(project.Ids[nameof(Project)], "TEST"), cancellationToken);
        if (layout.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test layout.");
        }

        CommandResponse layoutVersion = await sender.Send(new CreateItemLayoutVersionDb.Command(layout.Ids[nameof(GenericItemLayout)], "Test Layout", ItemLayoutUses.WorkItems, new Dictionary<uint, Guid>
        {
            { 0, field1v1.Ids[nameof(GenericItemFieldVersion)] },
            { 1, field2v1.Ids[nameof(GenericItemFieldVersion)] },
            { 2, field3v1.Ids[nameof(GenericItemFieldVersion)] },
            { 3, field4v1.Ids[nameof(GenericItemFieldVersion)] },
            { 4, field5v1.Ids[nameof(GenericItemFieldVersion)] },
            { 5, field6v1.Ids[nameof(GenericItemFieldVersion)] },
            { 6, field7v1.Ids[nameof(GenericItemFieldVersion)] }
        }), cancellationToken);

        if (layoutVersion.Ids.Count == 0)
        {
            return CommandResponse.Fail("Failed to create test layout version.");
        }

        for (int i = 0; i < 100; i++)
        {
            CommandResponse testWorkItem = await sender.Send(new CreateWorkItemDb.Command(queue.Ids[nameof(WorkItemQueue)], layout.Ids[nameof(GenericItemLayout)], DateTimeOffset.UtcNow), cancellationToken);
            if (testWorkItem.Ids.Count == 0)
            {
                return CommandResponse.Fail("Failed to create test work item.");
            }

            CommandResponse testWorkItemVersion = await sender.Send(new CreateWorkItemVersionDb.Command(testWorkItem.Ids[nameof(WorkItem)], "Test", layoutVersion.Ids[nameof(GenericItemLayoutVersion)], new Dictionary<Guid, object?>
            {
                { field1v1.Ids[nameof(GenericItemFieldVersion)], "Test Value" },
                { field2v1.Ids[nameof(GenericItemFieldVersion)], -123L },
                { field3v1.Ids[nameof(GenericItemFieldVersion)], 3.14d },
                { field4v1.Ids[nameof(GenericItemFieldVersion)], "Test Value 1" },
                { field5v1.Ids[nameof(GenericItemFieldVersion)], "Large Text\n\n\n\n\n\n\n\n\nLarge Text" },
                { field6v1.Ids[nameof(GenericItemFieldVersion)], true },
                { field7v1.Ids[nameof(GenericItemFieldVersion)], DateTimeOffset.UtcNow }
            }, DateTimeOffset.UtcNow), cancellationToken);

            if (testWorkItemVersion.Ids.Count == 0)
            {
                return CommandResponse.Fail("Failed to create test work item version.");
            }
        }

        return CommandResponse.Pass(nameof(Project), project.Ids[nameof(Project)]);
    }
}