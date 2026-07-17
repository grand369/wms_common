namespace Wms.Shared.Domain.ValueObjects;

/// <summary>
/// Quantity Value Object (VO-01) — immutable representation of a quantity with unit context.
/// Core to Inventory, Inbound, Outbound, Transfer, CycleCount modules.
/// </summary>
public readonly record struct Quantity(decimal Value, string UnitCode, string UnitName)
{
    public Quantity() : this(0m, string.Empty, string.Empty) { }

    public static Quantity Zero => new(0m, string.Empty, string.Empty);

    public bool IsZero => Value == 0m;

    public bool IsNegative => Value < 0m;

    public bool IsPositive => Value > 0m;

    public static Quantity Create(decimal value, string unitCode, string unitName)
    {
        if (value < 0m)
            throw new ArgumentException("Quantity value cannot be negative.", nameof(value));
        if (string.IsNullOrWhiteSpace(unitCode))
            throw new ArgumentException("Unit code cannot be empty.", nameof(unitCode));
        return new Quantity(value, unitCode.Trim(), unitName?.Trim() ?? string.Empty);
    }

    public Quantity Add(Quantity other)
    {
        if (UnitCode != other.UnitCode && !string.IsNullOrEmpty(UnitCode) && !string.IsNullOrEmpty(other.UnitCode))
            throw new InvalidOperationException($"Cannot add quantities with different units: {UnitCode} vs {other.UnitCode}");
        return new Quantity(Value + other.Value, UnitCode, UnitName);
    }

    public Quantity Subtract(Quantity other)
    {
        if (UnitCode != other.UnitCode && !string.IsNullOrEmpty(UnitCode) && !string.IsNullOrEmpty(other.UnitCode))
            throw new InvalidOperationException($"Cannot subtract quantities with different units: {UnitCode} vs {other.UnitCode}");
        return new Quantity(Value - other.Value, UnitCode, UnitName);
    }

    public override string ToString() => $"{Value} {UnitName}";
}
