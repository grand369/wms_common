namespace Wms.Warehouse.Domain.Enums;

/// <summary>
/// Storage Environment Smart Enum — defines the environmental condition of a warehouse area.
/// (ENT-02, Phase 3 DDD Design)
/// </summary>
public sealed class StorageEnvironment : SmartEnum<StorageEnvironment, int>
{
    public static readonly StorageEnvironment Normal = new StorageEnvironment("Normal", 0, "常温");
    public static readonly StorageEnvironment ColdChain = new StorageEnvironment("ColdChain", 1, "冷链");
    public static readonly StorageEnvironment ConstantTemp = new StorageEnvironment("ConstantTemp", 2, "恒温");
    public static readonly StorageEnvironment MoistureProof = new StorageEnvironment("MoistureProof", 3, "防潮");
    public static readonly StorageEnvironment DustProof = new StorageEnvironment("DustProof", 4, "防尘");

    public string Description { get; }

    private StorageEnvironment(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
