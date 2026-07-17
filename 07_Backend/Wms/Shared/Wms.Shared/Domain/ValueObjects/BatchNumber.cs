namespace Wms.Shared.Domain.ValueObjects;

/// <summary>
/// Batch Number Value Object (VO-05) — immutable identifier for material batches.
/// Shared across Inventory, Inbound, Outbound, CycleCount modules.
/// </summary>
public readonly record struct BatchNumber(string Value)
{
    public BatchNumber() : this(string.Empty) { }

    public static BatchNumber Empty => new(string.Empty);

    public static BatchNumber Unspecified => new("N/A");

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public bool IsUnspecified => Value == "N/A";

    public override string ToString() => Value;

    public static BatchNumber Create(string batchNo)
    {
        if (string.IsNullOrWhiteSpace(batchNo))
            return Unspecified;
        return new BatchNumber(batchNo.Trim());
    }
}
