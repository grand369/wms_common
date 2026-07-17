using System.Reflection;

namespace Wms.TestBase;

/// <summary>
/// WmsTestDataBuilder — base class for test data construction.
/// Provides helpers for generating common test identifiers and patterns.
/// Each module can subclass this to build module-specific test data.
/// (Phase 10 test standardization — Test Data Builder pattern)
/// </summary>
public abstract class WmsTestDataBuilder
{
    /// <summary>
    /// Generate a deterministic test Guid from a seed string.
    /// Useful for reproducible test data across runs.
    /// </summary>
    protected static Guid GuidFromSeed(string seed)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(seed);
        var guidBytes = new byte[16];
        for (int i = 0; i < 16 && i < bytes.Length; i++)
            guidBytes[i] = bytes[i];
        // Pad remaining bytes with seed-derived values
        for (int i = bytes.Length; i < 16; i++)
            guidBytes[i] = (byte)(i % 256);
        // Set RFC 4122 version 4 bits
        guidBytes[7] = (byte)((guidBytes[7] & 0x0F) | 0x40);
        guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80);
        return new Guid(guidBytes);
    }

    /// <summary>
    /// Generate a unique test code with a prefix.
    /// Format: {prefix}-{timestamp}-{random}
    /// </summary>
    protected static string TestCode(string prefix) => $"{prefix}-{DateTime.UtcNow.Ticks}-{Random.Shared.Next(1000, 9999)}";

    /// <summary>
    /// Generate a standard test warehouse ID (deterministic).
    /// </summary>
    protected static Guid TestWarehouseId => GuidFromSeed("TestWarehouse");

    /// <summary>
    /// Generate a standard test material ID (deterministic).
    /// </summary>
    protected static Guid TestMaterialId => GuidFromSeed("TestMaterial");

    /// <summary>
    /// Generate a standard test location ID (deterministic).
    /// </summary>
    protected static Guid TestLocationId => GuidFromSeed("TestLocation");

    /// <summary>
    /// Standard test user ID for authorization tests.
    /// </summary>
    protected static Guid TestUserId => GuidFromSeed("TestUser");
}
