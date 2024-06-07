namespace Bones.Database;

/// <summary>
///   Exceptions from the Database.
/// </summary>
/// <param name="reason">What went wrong.</param>
public class BonesDatabaseException(string reason) : ApplicationException(reason);
