namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Balance Freeze DTO — request body for freezing a specific inventory balance.
/// Simple freeze operation: balanceId + qty + reason.
/// </summary>
public class InventoryBalanceFreezeDto
{
    public decimal Qty { get; set; }
    public string Reason { get; set; } = string.Empty;
}
