using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Controllers;

/// <summary>
///   Almost entirely based on this standard implementation from the ASP.NET Core source,
///   linked to the exact commit, so I can track any changes that are made to this and update mine later:
///   https://github.com/dotnet/aspnetcore/blob/0d0aa336f66255111410454285b7172f86591fb3/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
/// </summary>
/// <remarks>
///   This is split into multiple files so it's less of a mess to read.
/// </remarks>
/// <param name="sender"></param>
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