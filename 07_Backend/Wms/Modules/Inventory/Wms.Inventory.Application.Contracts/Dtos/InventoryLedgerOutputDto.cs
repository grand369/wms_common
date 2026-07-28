namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Ledger Entry Output DTO — immutable read-only view of ledger records.
/// </summary>
public class InventoryLedgerOutputDto
{
    public Guid Id { get; set; }
    public Guid InventoryBalanceId { get; set; }
    
    // Material info
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    
    // Warehouse/Location info
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    
    // Operation info
    public int OperationTypeValue { get; set; }
    public string OperationTypeName { get; set; } = string.Empty;
    public decimal OperationQuantity { get; set; }
    public decimal InQuantity { get; set; }
    public decimal OutQuantity { get; set; }
    public decimal BalanceQuantity { get; set; }
    public decimal BeforeQuantity { get; set; }
    public decimal AfterQuantity { get; set; }
    public decimal BeforeAvailable { get; set; }
    public decimal AfterAvailable { get; set; }
    public DateTime OperationTime { get; set; }
    public Guid OperatorId { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    
    // Source order info
    public string SourceOrderType { get; set; } = string.Empty;
    public Guid SourceOrderId { get; set; }
    public string SourceOrderNo { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
}
