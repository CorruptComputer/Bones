using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Database.Operations.GenericItem;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.Operations.System.TestingDataSetup;

/// <inheritdoc />
public class SetupTestingDataDb(ISender sender)
    : IRequestHandler<SetupTestingDataDb.Command, CommandResponse>
{
    /// <summary>
    ///   Command for setting up default testing data
    ///   TODO: break this up into smaller commands to be more manageable
    /// </summary>
    public sealed record Command : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {                                                                 // TODO: Maybe make this a const?
        BonesUser? adminUser = await sender.Send(new GetUserByEmailDb.Query("admin@example.com"), cancellationToken);
        if (adminUser == null)
        {
            return CommandResponse.Fail("No admin user found. Cannot set up testing data.");
        }

        // Create a project under the default admin user
        CommandResponse project = await sender.Send(new CreateProjectDb.Command("Test Project", adminUser), cancellationToken);
        if (project.Id == null)
        {
            return CommandResponse.Fail("Failed to create test project.");
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

        // Add field versions
        CommandResponse field1v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field1.Id.Value, "Test Required Text", true, FieldType.Text, null, null, null, null), cancellationToken);
        if (field1v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 1 version 1.");
        }

        CommandResponse field2v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field2.Id.Value, "Test Required Negative Integer", true, FieldType.Integer, true, null, null, null), cancellationToken);
        if (field2v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 2 version 1.");
        }

        CommandResponse field3v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field3.Id.Value, "Test Optional Positive Decimal", false, FieldType.Decimal, false, null, null, null), cancellationToken);
        if (field3v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 3 version 1.");
        }

        CommandResponse field4v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field4.Id.Value, "Test Required Value List", true, FieldType.ValueList, null, new Dictionary<string, StringValueMatchingType>
        {
            { "Test Value 1", StringValueMatchingType.Exact },
            { "Test Value 2", StringValueMatchingType.CaseInvariant }
        }, null, null), cancellationToken);
        if (field4v1.Id == null)
        {
            return CommandResponse.Fail("Failed to create test field 4 version 1.");
        }

        CommandResponse field5v1 = await sender.Send(new CreateItemFieldVersionDb.Command(field5.Id.Value, "Test Optional GeoLocation", false, FieldType.GeoLocation, null, null, GeoLocationType.LatLon, null), cancellationToken);
        if (field5v1.Id == null)
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
            { 1, field1v1.Id.Value },
            { 2, field2v1.Id.Value },
            { 3, field3v1.Id.Value },
            { 4, field4v1.Id.Value },
            { 5, field5v1.Id.Value }
        }), cancellationToken);

        if (layoutVersion.Id == null)
        {
            return CommandResponse.Fail("Failed to create test layout version.");
        }

        return CommandResponse.Pass();
    }
}