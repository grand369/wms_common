namespace Wms.Material.Domain.Enums;

/// <summary>
/// ERP Sync Status Smart Enum — defines the ERP synchronization status of a material.
/// (ENT-04, Phase 3 DDD Design)
/// </summary>
public sealed class ErpSyncStatus : SmartEnum<ErpSyncStatus, int>
{
    public static readonly ErpSyncStatus None = new ErpSyncStatus("None", 0, "未同步");
    public static readonly ErpSyncStatus Synced = new ErpSyncStatus("Synced", 1, "已同步");
    public static readonly ErpSyncStatus Conflict = new ErpSyncStatus("Conflict", 2, "冲突");
    public static readonly ErpSyncStatus Pending = new ErpSyncStatus("Pending", 3, "待同步");

    public string Description { get; }

    private ErpSyncStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
