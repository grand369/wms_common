namespace Wms.Shared.Domain.ValueObjects;

/// <summary>
/// Material Code Value Object (VO-04) — immutable identifier for materials.
/// Shared across Warehouse, Material, Inventory, Inbound, Outbound modules.
/// </summary>
public readonly record struct MaterialCode(string Value)
{
    public MaterialCode() : this(string.Empty) { }

    public static MaterialCode Empty => new(string.Empty);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;

    public static MaterialCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Material code cannot be empty.", nameof(code));
        return new MaterialCode(code.Trim());
    }
}
