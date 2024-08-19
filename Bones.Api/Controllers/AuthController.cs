using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Controllers;

public sealed partial class AuthController(ISender sender) : BonesControllerBase(sender)
{
    private static ValidationProblem CreateValidationProblem(IdentityResult? result)
    {
        Dictionary<string, string[]> errorDictionary = new(1);

        if (result == null)
        {
            return TypedResults.ValidationProblem(errorDictionary);
        }

        foreach (IdentityError error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out string[]? descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }


        return TypedResults.ValidationProblem(errorDictionary);
    }
}