namespace Bones.Shared.Models;

public interface IValidatableRequest<out TResponse> : IRequest<TResponse>
{
    /// <summary>
    ///   Should perform basic sanity checks on the input values for the request,
    ///   no DB or external calls should be made here, just look at the raw values.
    /// </summary>
    /// <returns>true if valid, false if invalid</returns>
    public (bool valid, string? invalidReason) IsRequestValid();
}