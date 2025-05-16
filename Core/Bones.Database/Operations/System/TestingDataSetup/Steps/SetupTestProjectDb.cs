using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Database.Operations.GenericItem;
using Bones.Database.Operations.ProjectManagement.Initiatives;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
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

        if (project.Id == null)
        {
            return CommandResponse.Fail("Failed to create test project.");
        }

        CommandResponse initiative = await sender.Send(new CreateInitiativeDb.Command("Test Initiative", project.Id.Value), cancellationToken);
        if (initiative.Id == null)
        {
            return CommandResponse.Fail("Failed to create test initiative.");
        }

        CommandResponse queue = await sender.Send(new CreateWorkItemQueueDb.Command("Test Queue", initiative.Id.Value), cancellationToken);
        if (queue.Id == null)
        {
            return CommandResponse.Fail("Failed to create test queue.");
        }

        // Add fields
        CommandResponse field1 = await sender.Send(new CreateItemFieldDb.Command(project.Id.Value), cancellationToken);
        if (field1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 1.");
        }

        CommandResponse field2 = await sender.Send(new CreateItemFieldDb.Command(project.Id.Value), cancellationToken);
        if (field2.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 2.");
        }

        CommandResponse field3 = await sender.Send(new CreateItemFieldDb.Command(project.Id.Value), cancellationToken);
        if (field3.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 3.");
        }

        CommandResponse field4 = await sender.Send(new CreateItemFieldDb.Command(project.Id.Value), cancellationToken);
        if (field4.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 4.");
        }

        CommandResponse field5 = await sender.Send(new CreateItemFieldDb.Command(project.Id.Value), cancellationToken);
        if (field5.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 5.");
        }

        CommandResponse field6 = await sender.Send(new CreateItemFieldDb.Command(project.Id.Value), cancellationToken);
        if (field6.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 6.");
        }

        CommandResponse field7 = await sender.Send(new CreateItemFieldDb.Command(project.Id.Value), cancellationToken);
        if (field7.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 7.");
        }

        // Add field versions
        CommandResponse field1v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field1.Id.Value, "Required Small Text", true, FieldType.TextField, null, null, null, null), cancellationToken);
        if (field1v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 1 version 1.");
        }

        CommandResponse field2v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field2.Id.Value, "Required Integer", true, FieldType.Integer, true, null, null, null), cancellationToken);
        if (field2v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 2 version 1.");
        }

        CommandResponse field3v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field3.Id.Value, "Optional Positive Decimal", false, FieldType.Decimal, false, null, null, null), cancellationToken);
        if (field3v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 3 version 1.");
        }

        CommandResponse field4v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field4.Id.Value, "Required Value List", true, FieldType.ValueList, null, new Dictionary<string, StringValueMatchingType>
        {
            { "Test Value 1", StringValueMatchingType.Exact },
            { "Test Value 2", StringValueMatchingType.CaseInvariant }
        }, null, null), cancellationToken);
        if (field4v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 4 version 1.");
        }

        CommandResponse field5v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field5.Id.Value, "Optional Large Text", false, FieldType.TextBox, null, null, null, null), cancellationToken);
        if (field5v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 5 version 1.");
        }

        CommandResponse field6v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field6.Id.Value, "Optional Boolean", false, FieldType.Boolean, null, null, null, null), cancellationToken);
        if (field6v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 5 version 1.");
        }

        CommandResponse field7v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field7.Id.Value, "Optional DateTime", false, FieldType.DateTime, null, null, null, null), cancellationToken);
        if (field7v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 5 version 1.");
        }

        // Add a layout to the project
        CommandResponse layout = await sender.Send(new CreateItemLayoutDb.Command(project.Id.Value, "TST1"), cancellationToken);
        if (layout.Id == null)
        {
            return CommandResponse.Fail("Failed to create test layout.");
        }

        CommandResponse layoutVersion = await sender.Send(new CreateItemLayoutVersionDb.Command(layout.Id.Value, "Test Layout Version 1", ItemLayoutUses.WorkItems, new Dictionary<uint, Guid>
        {
            { 0, field1v1.Id.Value },
            { 1, field2v1.Id.Value },
            { 2, field3v1.Id.Value },
            { 3, field4v1.Id.Value },
            { 4, field5v1.Id.Value },
            { 5, field6v1.Id.Value },
            { 6, field7v1.Id.Value }
        }), cancellationToken);

        if (layoutVersion.Id == null)
        {
            return CommandResponse.Fail("Failed to create test layout version.");
        }

        return CommandResponse.Pass(project.Id);
    }
}