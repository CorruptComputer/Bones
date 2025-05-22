namespace Bones.Shared.Extensions;

/// <summary>
///   Extensions for <see cref="DateTimeOffset"/>
/// </summary>
public static class DateTimeOffsetExtensions
{
    /// <summary>
    ///   Gets the <see cref="DateOnly"/> representation of the <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static DateOnly ToDateOnly(this DateTimeOffset dto)
    {
        return dto.DateTime.ToDateOnly();
    }

    /// <summary>
    ///   Gets the <see cref="TimeOnly"/> representation of the <see cref="DateTimeOffset"/>
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static TimeOnly ToTimeOnly(this DateTimeOffset dto)
    {
        return dto.DateTime.ToTimeOnly();
    }
}
