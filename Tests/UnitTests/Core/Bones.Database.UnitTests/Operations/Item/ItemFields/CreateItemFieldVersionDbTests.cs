using Bones.Database.DbSets.Items;
using Bones.Database.Operations.Item;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.Item.ItemFields;

/// <summary>
///   Tests for creating item field versions
/// </summary>
public class CreateItemFieldVersionDbTests : TestBase
{
    private readonly CreateItemFieldVersionDb.Validator _validator = new();

    /// <summary>
    ///   Test for creating a required text field
    /// </summary>
    [Fact]
    public async Task RequiredTextFieldVersion()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Required Text",
            true,
            FieldType.TextField,
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse response = await Sender.Send(createFieldVersionCommand);
        response.Success.ShouldBeTrue();
    }

    /// <summary>
    ///   Test for creating an optional text field
    /// </summary>
    [Fact]
    public async Task OptionalTextFieldVersion()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Optional Text",
            false,
            FieldType.TextField,
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse response = await Sender.Send(createFieldVersionCommand);
        response.Success.ShouldBeTrue();
    }

    /// <summary>
    ///   Test for creating a number field with constraints
    /// </summary>
    [Fact]
    public async Task NumberFieldWithConstraints()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Number Field",
            true,
            FieldType.Decimal,
            false, // CanBeNegative
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse response = await Sender.Send(createFieldVersionCommand);
        response.Success.ShouldBeTrue();
    }

    /// <summary>
    ///   Test for creating a date field with default value
    /// </summary>
    [Fact]
    public async Task DateFieldWithDefaultValue()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Date Field",
            true,
            FieldType.DateTime,
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse response = await Sender.Send(createFieldVersionCommand);
        response.Success.ShouldBeTrue();
    }

    /// <summary>
    ///   Test for creating a field with invalid constraints
    /// </summary>
    [Fact]
    public async Task InvalidConstraints_ShouldFail()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Invalid Field",
            true,
            FieldType.Decimal,
            null, // CanBeNegative is required for decimal fields
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldHaveValidationErrors();
    }

    /// <summary>
    ///   Test for creating a field with invalid name
    /// </summary>
    [Fact]
    public async Task InvalidName_ShouldFail()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            new('a', 513), // Name too long
            true,
            FieldType.TextBox,
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldHaveValidationErrors();
    }

    /// <summary>
    ///   Test for creating a field with invalid field type
    /// </summary>
    [Fact]
    public async Task InvalidFieldType_ShouldFail()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Invalid Field Type",
            true,
            (FieldType)999, // Invalid field type value
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldHaveValidationErrors();
    }

    /// <summary>
    ///   Test for creating a field with missing CanBeNegative for numeric type
    /// </summary>
    [Fact]
    public async Task MissingCanBeNegativeForNumericType_ShouldFail()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Numeric Field",
            true,
            FieldType.Decimal,
            null, // CanBeNegative is required for decimal fields
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldHaveValidationErrors();
    }

    /// <summary>
    ///   Test for creating a field with missing GeoLocation type
    /// </summary>
    [Fact]
    public async Task MissingGeoLocationType_ShouldFail()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Geo Field",
            true,
            FieldType.GeoLocation,
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldHaveValidationErrors();
    }

    /// <summary>
    ///   Test for creating a field with missing required address fields
    /// </summary>
    [Fact]
    public async Task MissingRequiredAddressFields_ShouldFail()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "GeoLocation Field",
            true,
            FieldType.GeoLocation,
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldHaveValidationErrors();
    }

    /// <summary>
    ///   Test for creating a field with missing possible values for value list type
    /// </summary>
    [Fact]
    public async Task MissingPossibleValuesForValueListType_ShouldFail()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            createFieldResponse.Ids[nameof(ItemField)],
            "Value List Field",
            true,
            FieldType.ValueList,
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldHaveValidationErrors();
    }

    /// <summary>
    ///   Test for creating a field version for a non-existent field
    /// </summary>
    [Fact]
    public async Task NonExistentField_ShouldFail()
    {
        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
            Guid.NewGuid(),
            "Non-existent Field",
            true,
            FieldType.TextBox,
            null,
            null,
            null,
            null
        );

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse response = await Sender.Send(createFieldVersionCommand);
        response.Success.ShouldBeFalse();
        response.FailureReasons.ShouldContainKey(BonesResponseBase.SERVER_ERROR_KEY);
        response.FailureReasons[BonesResponseBase.SERVER_ERROR_KEY].ShouldContain("Field not found");
    }

    /// <summary>
    ///   Test for creating multiple versions of a field
    /// </summary>
    [Fact]
    public async Task MultipleVersions_ShouldSucceed()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        // Create multiple versions
        for (int i = 0; i < 5; i++)
        {
            CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
                createFieldResponse.Ids[nameof(ItemField)],
                $"Version {i}",
                true,
                FieldType.TextBox,
                null,
                null,
                null,
                null
            );

            TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
            validationResult.ShouldNotHaveAnyValidationErrors();

            CommandResponse response = await Sender.Send(createFieldVersionCommand);
            response.Success.ShouldBeTrue();
        }
    }

    /// <summary>
    ///   Test for handling concurrent access
    /// </summary>
    [Fact]
    public async Task ConcurrentAccess_ShouldHandleCorrectly()
    {
        Guid projectId = await CreateEmptyProject("Test Project");

        // Create a field
        CreateItemFieldDb.Command createItemFieldCommand = new(projectId);
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);
        createFieldResponse.Success.ShouldBeTrue();
        createFieldResponse.Ids.ShouldNotBeEmpty();

        // Create multiple versions concurrently
        Task<CommandResponse>[] tasks = Enumerable.Range(0, 5)
            .Select(i =>
            {
                CreateItemFieldVersionDb.Command createFieldVersionCommand = new(
                    createFieldResponse.Ids[nameof(ItemField)],
                    $"Version {i}",
                    true,
                    FieldType.TextField,
                    null,
                    null,
                    null,
                    null
                );

                return Sender.Send(createFieldVersionCommand);
            })
            .ToArray();

        CommandResponse[] results = await Task.WhenAll(tasks);

        // All should succeed since we're creating new versions
        results.All(r => r.Success).ShouldBeTrue();
    }
}
