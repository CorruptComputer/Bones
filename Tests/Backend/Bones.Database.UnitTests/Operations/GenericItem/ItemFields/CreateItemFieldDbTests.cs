using Bones.Database.Operations.GenericItem;
using Bones.Shared.Backend.Models;


namespace Bones.Database.UnitTests.Operations.GenericItem.ItemFields;

public class CreateItemFieldDbTests : TestBase
{
    private readonly CreateItemFieldDb.Validator _validator = new();

    /// <summary>
    ///     Checks that the handler stops this.
    /// </summary>
    [Fact]
    public async Task EmptyProjectId_ShouldFailValidation()
    {
        CreateItemFieldDb.Command emptyProjectIdCommand = new(Guid.Empty);
        TestValidationResult<CreateItemFieldDb.Command> validationResult = await _validator.TestValidateAsync(emptyProjectIdCommand);
        validationResult.ShouldHaveAnyValidationError();

        CommandResponse response = await Sender.Send(emptyProjectIdCommand);
        response.Success.Should().BeFalse();
    }
}
