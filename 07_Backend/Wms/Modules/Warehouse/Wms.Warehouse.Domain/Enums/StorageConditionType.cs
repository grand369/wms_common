namespace Wms.Warehouse.Domain.Enums;

/// <summary>
/// Storage Condition Type Smart Enum — defines the storage condition requirements.
/// Warehouse module's own version (same concept as shared StorageCondition but scoped to Warehouse BC).
/// (ENT-01/ENT-03, Phase 3 DDD Design)
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
