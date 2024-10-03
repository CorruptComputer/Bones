namespace Bones.Database.Operations.GenericItem.ItemLayouts.UpdateItemLayoutByIdDb;

/// <summary>
///   DB Command for updating an Asset Layout
/// </summary>
/// <param name="ItemLayoutId"></param>
/// <param name="NewName"></param>
public sealed record UpdateItemLayoutByIdDbCommand(Guid ItemLayoutId, string NewName) : IRequest<CommandResponse>;