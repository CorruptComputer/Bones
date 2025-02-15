namespace Bones.Shared.Extensions;

/// <summary>
///   Extensions to object
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    ///   Checks if the objects type is any numeric type:
    ///   byte, ushort, short, uint, int, ulong, long, double, decimal, or float
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool IsNumericType(this object o)
    {
        return o.IsIntegerType() || o.IsDecimalType();
    }

    /// <summary>
    ///   Checks if the objects type is an integer type:
    ///   byte, ushort, short, uint, int, ulong, or long
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
        return Type.GetTypeCode(o.GetType()) switch
        {
            TypeCode.SByte
                or TypeCode.Int16
                or TypeCode.Int32
                or TypeCode.Int64 => true,
            _ => false
        };
    }

    /// <summary>
    ///   Checks if the objects type is an unsigned integer type:
    ///   byte, ushort, uint, or ulong
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool IsUnsignedIntegerType(this object o)
    {
        return Type.GetTypeCode(o.GetType()) switch
        {
            TypeCode.Byte
                or TypeCode.UInt16
                or TypeCode.UInt32
                or TypeCode.UInt64 => true,
            _ => false
        };
    }

    /// <summary>
    ///   Checks if the objects type is a decimal type:
    ///   double, decimal, or float
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static bool IsDecimalType(this object o)
    {
        return Type.GetTypeCode(o.GetType()) switch
        {
            TypeCode.Decimal
                or TypeCode.Double
                or TypeCode.Single => true,
            _ => false
        };
    }
}