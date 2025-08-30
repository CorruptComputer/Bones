using Bones.Testing.UnitTests.Shared;

namespace Bones.Shared.Backend.UnitTests;

/// <summary>
///   Tests for the <see cref="EncryptionHelper"/> class
/// </summary>
public class EncryptionHelperTests : TestBase
{
    /// <summary>
    ///   Encryption should be performed correctly
    /// </summary>
    [Fact]
    public void Encryption_ShouldWork()
    {
        string encryptionKey = EncryptionHelper.GenerateAESKey();
        encryptionKey.ShouldNotBeNullOrEmpty();
        encryptionKey.Length.ShouldBe(44); // 32 bytes in Base64 is 44 characters

        string encrypted = EncryptionHelper.Encrypt("test", encryptionKey);
        encrypted.ShouldNotBeNullOrEmpty();

        string decrypted = EncryptionHelper.Decrypt(encrypted, encryptionKey);
        decrypted.ShouldNotBeNullOrEmpty();
        decrypted.ShouldBe("test");
    }

    /// <summary>
    ///   Encryption should be performed correctly
    /// </summary>
    [Fact]
    public void Encryption_ShouldRandomizeIV()
    {
        string encryptionKey = EncryptionHelper.GenerateAESKey();
        encryptionKey.ShouldNotBeNullOrEmpty();
        encryptionKey.Length.ShouldBe(44); // 32 bytes in Base64 is 44 characters

        string encrypted1 = EncryptionHelper.Encrypt("test", encryptionKey);
        encrypted1.ShouldNotBeNullOrEmpty();

        string encrypted2 = EncryptionHelper.Encrypt("test", encryptionKey);
        encrypted2.ShouldNotBeNullOrEmpty();

        encrypted1.ShouldNotBe(encrypted2); // Different IVs should produce different ciphertexts
    }
}
