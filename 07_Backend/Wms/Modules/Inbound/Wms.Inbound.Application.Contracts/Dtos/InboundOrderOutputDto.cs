namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundOrderOutputDto — output DTO for inbound order, includes flattened line data.
/// </summary>
public class InboundOrderOutputDto
{
    public Guid Id { get; set; }
    public string InboundOrderNo { get; set; } = string.Empty;
    public int InboundTypeValue { get; set; }
    public string InboundTypeName { get; set; } = string.Empty;
    public int InboundStatusValue { get; set; }
    public string InboundStatusName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public Guid? PurchaseOrderId { get; set; }
    public string? PurchaseOrderNo { get; set; }
    public Guid? ProductionOrderId { get; set; }
    public Guid? ReturnOrderId { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public decimal OverReceiptRatio { get; set; }
    public bool QualityInspectionRequired { get; set; }
    public decimal TotalPlanQuantity { get; set; }
    public decimal TotalReceivedQuantity { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletionTime { get; set; }
    public int ErpCallbackStatusValue { get; set; }
    public string ErpCallbackStatusName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
    public List<InboundLineOutputDto> Lines { get; set; } = new();
}

/// <summary>
/// InboundLineOutputDto — flattened output DTO for an inbound line.
/// </summary>
public class InboundLineOutputDto
{
    public Guid Id { get; set; }
    public Guid InboundOrderId { get; set; }
    public int LineNo { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PlanQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string? BatchNumber { get; set; }
    public List<string>? SerialNumberList { get; set; }
    public int QualityStatusValue { get; set; }
    public string QualityStatusName { get; set; } = string.Empty;
    public Guid? PutawayWarehouseId { get; set; }
    public string? PutawayWarehouseCode { get; set; }
    public Guid? PutawayAreaId { get; set; }
    public string? PutawayAreaCode { get; set; }
    public Guid? PutawayLocationId { get; set; }
    public string? PutawayLocationCode { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ProductionDate { get; set; }
    public string? Remark { get; set; }
}
