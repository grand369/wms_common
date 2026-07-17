using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Enums;

/// <summary>
/// Inventory Operation Type SmartEnum — defines the 10 types of operations
/// that can affect inventory balance. (ENT-07, Phase 3 DDD Design)
/// </summary>
public sealed class InventoryOperationType : SmartEnum<InventoryOperationType, int>
{
    public static readonly InventoryOperationType InboundIncrease =
        new InventoryOperationType("InboundIncrease", 0, "入库增加");
    public static readonly InventoryOperationType OutboundDecrease =
        new InventoryOperationType("OutboundDecrease", 1, "出库扣减");
    public static readonly InventoryOperationType AdjustIncrease =
        new InventoryOperationType("AdjustIncrease", 2, "调整增加");
    public static readonly InventoryOperationType AdjustDecrease =
        new InventoryOperationType("AdjustDecrease", 3, "调整扣减");
    public static readonly InventoryOperationType Freeze =
        new InventoryOperationType("Freeze", 4, "冻结");
    public static readonly InventoryOperationType Unfreeze =
        new InventoryOperationType("Unfreeze", 5, "解冻");
    public static readonly InventoryOperationType TransferIn =
        new InventoryOperationType("TransferIn", 6, "调拨入库");
    public static readonly InventoryOperationType TransferOut =
        new InventoryOperationType("TransferOut", 7, "调拨出库");
    public static readonly InventoryOperationType BackflushDecrease =
        new InventoryOperationType("BackflushDecrease", 8, "倒推消耗");
    public static readonly InventoryOperationType ReplenishmentIncrease =
        new InventoryOperationType("ReplenishmentIncrease", 9, "补料入库");

    public string Description { get; }

    private InventoryOperationType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
