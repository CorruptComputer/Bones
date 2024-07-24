namespace Bones.Shared.Models;

public interface IValidatableRequestHandler<in TIn, TOut> : IRequestHandler<TIn, TOut> where TIn: IRequest<TOut>
{
    /// <summary>
    ///   Should perform basic sanity checks on the input values for the request,
    ///   no DB or external calls should be made here, just look at the raw values.
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>true if valid, false if invalid</returns>
    public bool RequestIsValid(TIn request);
}