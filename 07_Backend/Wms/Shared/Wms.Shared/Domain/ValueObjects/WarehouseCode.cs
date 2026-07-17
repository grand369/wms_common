namespace Wms.Shared.Domain.ValueObjects;

/// <summary>
/// Warehouse Code Value Object (VO-02) — immutable identifier for warehouses.
/// Shared across Warehouse, Inventory, Inbound, Outbound, Transfer, TaskCenter modules.
/// </summary>
public readonly record struct WarehouseCode(string Value)
{
    public WarehouseCode() : this(string.Empty) { }

    public static WarehouseCode Empty => new(string.Empty);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;

    public static WarehouseCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Warehouse code cannot be empty.", nameof(code));
        return new WarehouseCode(code.Trim());
    }
}
