using Bones.Shared.Exceptions;

namespace Bones.Api;

internal class BadRequestException(string message = "Bad Request") : BonesException(message)
{
    internal required string RequestModel { get; init; } = string.Empty;
    internal required string BadField { get; init; } = string.Empty;
}
