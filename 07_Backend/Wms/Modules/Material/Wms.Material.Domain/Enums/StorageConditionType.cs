namespace Wms.Material.Domain.Enums;

/// <summary>
/// Storage Condition Type Smart Enum — defines the storage condition requirements for materials.
/// Material module's own version (same concept as Warehouse's StorageConditionType, scoped to Material BC).
/// Values are aligned with Warehouse BC's StorageConditionType for cross-module compatibility.
/// (VO-11, Phase 3 DDD Design)
/// </summary>
public sealed class StorageConditionType : SmartEnum<StorageConditionType, int>
{
    public static readonly StorageConditionType Normal = new StorageConditionType("Normal", 0, "常温");
    public static readonly StorageConditionType ColdChain = new StorageConditionType("ColdChain", 1, "冷链");
    public static readonly StorageConditionType ConstantTemp = new StorageConditionType("ConstantTemp", 2, "恒温");
    public static readonly StorageConditionType MoistureProof = new StorageConditionType("MoistureProof", 3, "防潮");
    public static readonly StorageConditionType DustProof = new StorageConditionType("DustProof", 4, "防尘");

    public string Description { get; }

    private StorageConditionType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
