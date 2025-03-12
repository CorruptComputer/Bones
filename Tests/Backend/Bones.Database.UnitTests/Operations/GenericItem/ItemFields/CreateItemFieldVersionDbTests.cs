using Bones.Database.Operations.GenericItem;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Backend.Models;

namespace Bones.Database.UnitTests.Operations.GenericItem.ItemFields;

/// <summary>
///   Tests for creating item field versions
/// </summary>
public class CreateItemFieldVersionDbTests : TestBase
{
    private readonly CreateItemFieldVersionDb.Validator _validator = new();

    /// <summary>
    ///   Test for creating a required text field
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "Requires a project, which requires a user")]
    public async Task RequiredTextFieldVersion()
    {
        CreateItemFieldDb.Command createItemFieldCommand = new(Guid.NewGuid());
        CommandResponse createFieldResponse = await Sender.Send(createItemFieldCommand);

        CreateItemFieldVersionDb.Command createFieldVersionCommand = new(createFieldResponse.Id!.Value, "Required Text", true, FieldType.Text, null, null, null, null);

        TestValidationResult<CreateItemFieldVersionDb.Command> validationResult = await _validator.TestValidateAsync(createFieldVersionCommand);
        validationResult.ShouldNotHaveAnyValidationErrors();

        CommandResponse response = await Sender.Send(createFieldVersionCommand);
        response.Success.Should().BeTrue();
    }
}
