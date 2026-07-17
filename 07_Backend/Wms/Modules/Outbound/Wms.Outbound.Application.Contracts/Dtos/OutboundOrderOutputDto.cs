namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// OutboundOrderOutputDto — output DTO for outbound order, includes flattened line data.
/// SmartEnum fields are flattened to int Value + string Description (DTO-OUT-003).
/// </summary>
public class OutboundOrderOutputDto
{
    public Guid Id { get; set; }
    public string OutboundOrderNo { get; set; } = string.Empty;
    public int OutboundTypeValue { get; set; }
    public string OutboundTypeName { get; set; } = string.Empty;
    public int OutboundStatusValue { get; set; }
    public string OutboundStatusName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public Guid? MaterialRequisitionId { get; set; }
    public Guid? SalesOrderId { get; set; }
    public Guid? ReturnMaterialOrderId { get; set; }
    public decimal OverIssueRatio { get; set; }
    public bool IsEmergency { get; set; }
    public decimal TotalRequiredQuantity { get; set; }
    public decimal TotalAllocatedQuantity { get; set; }
    public decimal TotalPickedQuantity { get; set; }
    public decimal TotalShippedQuantity { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletionTime { get; set; }
    public int ErpCallbackStatusValue { get; set; }
    public string ErpCallbackStatusName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
    public List<OutboundLineOutputDto> Lines { get; set; } = new();
}

/// <summary>
/// OutboundLineOutputDto — flattened output DTO for an outbound line.
/// </summary>
public class OutboundLineOutputDto
{
    public Guid Id { get; set; }
    public Guid OutboundOrderId { get; set; }
    public int LineNo { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal PickedQuantity { get; set; }
    public decimal ShippedQuantity { get; set; }
    public Guid? PickingLocationId { get; set; }
    public string? PickingLocationCode { get; set; }
    public int IssueStrategyValue { get; set; }
    public string IssueStrategyName { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public string? Remark { get; set; }
}
