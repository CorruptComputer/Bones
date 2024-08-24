using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;

public sealed record GetUserByClaimsPrincipalRequest : IRequest<QueryResponse<BonesUser>>
{
    public ClaimsPrincipal? ClaimsPrincipal { get; set; }
}