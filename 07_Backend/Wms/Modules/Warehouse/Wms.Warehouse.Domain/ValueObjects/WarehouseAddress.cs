namespace Wms.Warehouse.Domain.ValueObjects;

/// <summary>
/// Warehouse Address Value Object — represents the physical address of a warehouse.
/// Simplified version with Address, City, Province, Country fields.
/// Embedded as Owned Entity in Warehouse aggregate.
/// (ENT-01, Phase 3 DDD Design)
/// </summary>
public record WarehouseAddress
{
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;

    public WarehouseAddress() { }

    public WarehouseAddress(string address, string city = "", string province = "", string country = "CN")
    {
        Address = address ?? string.Empty;
        City = city ?? string.Empty;
        Province = province ?? string.Empty;
        Country = country ?? "CN";
    }

    public string FullAddress => $"{Country} {Province} {City} {Address}".Trim();

    public bool IsEmpty => string.IsNullOrWhiteSpace(Address)
        && string.IsNullOrWhiteSpace(City)
        && string.IsNullOrWhiteSpace(Province);
}
