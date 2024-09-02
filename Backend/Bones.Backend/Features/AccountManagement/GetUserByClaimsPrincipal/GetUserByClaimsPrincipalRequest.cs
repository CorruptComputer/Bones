using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;

/// <summary>
///   Backend request for getting a <see cref="BonesUser" /> by a <see cref="ClaimsPrincipal" />.
/// </summary>
/// <param name="ClaimsPrincipal"></param>
public sealed record GetUserByClaimsPrincipalRequest(ClaimsPrincipal? ClaimsPrincipal) : IRequest<QueryResponse<BonesUser>>;