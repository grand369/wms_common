namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Alert Query DTO — parameters for filtering alert queries.
/// </summary>
public class InventoryAlertQueryDto
{
    public int? AlertTypeValue { get; set; }
    public Guid? MaterialId { get; set; }
    public bool? IsResolved { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
