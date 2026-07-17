namespace Wms.Shared.Domain.Helpers;

/// <summary>
/// ID Generator — provides sequential GUID generation for entity primary keys.
/// Uses a time-based algorithm to generate sortable GUIDs that work well with SQL Server clustered indexes.
/// </summary>
public static class IdGenerator
{
    /// <summary>
    /// Generates a new sequential GUID suitable for SQL Server clustered index usage.
    /// The GUID is time-based and sortable, avoiding random GUID page fragmentation.
    /// </summary>
    public static Guid NewSequentialId()
    {
        var timestamp = DateTime.UtcNow.Ticks / 10000L; // milliseconds since year 0
        var guidBytes = Guid.NewGuid().ToByteArray();

        // Replace the first 8 bytes with the timestamp for sortability
        var timestampBytes = BitConverter.GetBytes(timestamp);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timestampBytes);
        }

        for (int i = 0; i < 8; i++)
        {
            guidBytes[i] = timestampBytes[i];
        }

        return new Guid(guidBytes);
    }

    /// <summary>
    /// Generates a business order number with module prefix and timestamp.
    /// Format: {Prefix}-{YYMMDDHHmmss}-{RandomSuffix}
    /// Example: IN-250715143000-A3F2
    /// </summary>
    public static string NewOrderNo(string prefix, int randomSuffixLength = 4)
    {
        var timestampPart = DateTime.UtcNow.ToString("yyMMddHHmmss");
        var randomPart = GenerateRandomSuffix(randomSuffixLength);
        return $"{prefix}-{timestampPart}-{randomPart}";
    }

    private static string GenerateRandomSuffix(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }
}
