namespace Wms.Shared.Domain.ValueObjects;

/// <summary>
/// Location Code Value Object (VO-03) — immutable identifier for storage locations.
/// Shared across Warehouse, Inventory, Inbound, Outbound modules.
/// </summary>
public readonly record struct LocationCode(string Value)
{
    public LocationCode() : this(string.Empty) { }

    public static LocationCode Empty => new(string.Empty);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;

    public static LocationCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Location code cannot be empty.", nameof(code));
        return new LocationCode(code.Trim());
    }
}
