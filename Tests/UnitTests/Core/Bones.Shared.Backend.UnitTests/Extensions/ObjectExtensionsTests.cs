using Bones.Shared.Extensions;
using Bones.Testing.UnitTests.Shared;

namespace Bones.Shared.Backend.UnitTests.Extensions;

/// <summary>
/// 
/// </summary>
public class ObjectExtensionsTests : TestBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void SignedIntegerTypes_ShouldBeCorrectlyIdentified()
    {
        sbyte sbyteValue = 123;
        short shortValue = 123;
        int intValue = 123;
        long longValue = 123;

        sbyteValue.IsSignedIntegerType().ShouldBeTrue();
        shortValue.IsSignedIntegerType().ShouldBeTrue();
        intValue.IsSignedIntegerType().ShouldBeTrue();
        longValue.IsSignedIntegerType().ShouldBeTrue();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void UnsignedIntegerTypes_ShouldBeCorrectlyIdentified()
    {
        byte byteValue = 123;
        ushort ushortValue = 123;
        uint uintValue = 123;
        ulong ulongValue = 123;

        byteValue.IsUnsignedIntegerType().ShouldBeTrue();
        ushortValue.IsUnsignedIntegerType().ShouldBeTrue();
        uintValue.IsUnsignedIntegerType().ShouldBeTrue();
        ulongValue.IsUnsignedIntegerType().ShouldBeTrue();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void DecimalTypes_ShouldBeCorrectlyIdentified()
    {
        double doubleValue = 123.45;
        decimal decimalValue = 123.45M;
        float floatValue = 123.45F;

        doubleValue.IsDecimalType().ShouldBeTrue();
        decimalValue.IsDecimalType().ShouldBeTrue();
        floatValue.IsDecimalType().ShouldBeTrue();
    }
}
