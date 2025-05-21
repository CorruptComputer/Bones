namespace Bones.Shared.Extensions;

/// <summary>
///   Extensions to object
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    ///   Checks if the objects type is any numeric type:
    ///   sbyte, short, int, long,
    ///   byte, ushort, uint, ulong,
    ///   double, decimal, or float
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool IsNumericType(this object o)
    {
        return o.IsIntegerType() || o.IsDecimalType();
    }

    /// <summary>
    ///   Checks if the objects type is an integer type:
    ///   sbyte, short, int, long,
    ///   byte, ushort, uint, or ulong
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool IsIntegerType(this object o)
    {
        return o.IsSignedIntegerType() || o.IsUnsignedIntegerType();
    }

    /// <summary>
    ///   Checks if the objects type is a signed integer type:
    ///   sbyte, short, int, or long
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool IsSignedIntegerType(this object o)
    {
        if (o is sbyte or short or int or long)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   Checks if the objects type is an unsigned integer type:
    ///   byte, ushort, uint, or ulong
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool IsUnsignedIntegerType(this object o)
    {
        if (o is byte or ushort or uint or ulong)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   Checks if the objects type is a decimal type:
    ///   double, decimal, or float
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool IsDecimalType(this object o)
    {
        if (o is double or decimal or float)
        {
            return true;
        }

        return false;
    }
}