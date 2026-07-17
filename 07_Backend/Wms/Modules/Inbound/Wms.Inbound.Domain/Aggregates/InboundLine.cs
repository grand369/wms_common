using Volo.Abp.Domain.Entities.Auditing;
using Wms.Inbound.Domain.Enums;

namespace Wms.Inbound.Domain.Aggregates;

/// <summary>
/// InboundLine Child Entity (ENT-08a) — nested within InboundOrder aggregate.
/// Represents a single material line in an inbound receipt order.
/// Inherits FullAuditedEntity<Guid> (not aggregate root — lifecycle bound to parent).
/// (AGG-11, Phase 3 DDD Design)
/// </summary>
public class InboundLine : FullAuditedEntity<Guid>
{
    /// <summary>Parent inbound order ID — foreign key.</summary>
    public Guid InboundOrderId { get; private set; }

    /// <summary>Line number — sequential within the order.</summary>
    public int LineNo { get; private set; }

    /// <summary>Material ID.</summary>
    public Guid MaterialId { get; private set; }

    /// <summary>Material code — redundant for query optimization.</summary>
    public string MaterialCode { get; private set; }

    /// <summary>Material name — redundant for query optimization.</summary>
    public string MaterialName { get; private set; }

    /// <summary>Plan quantity — expected receipt quantity.</summary>
    public decimal PlanQuantity { get; private set; }

    /// <summary>Received quantity — actual receipt quantity (default 0).</summary>
    public decimal ReceivedQuantity { get; private set; }

    /// <summary>Batch number — nullable, required for batch-managed materials.</summary>
    public string? BatchNumber { get; private set; }

    /// <summary>Serial number list — JSON stored, required for serial-managed materials.</summary>
    public List<string>? SerialNumberList { get; private set; }

    /// <summary>Quality status — Pending/Qualified/Unqualified/Skip.</summary>
    public QualityStatus QualityStatus { get; private set; }

    /// <summary>Putaway location ID — nullable, set during putaway confirmation.</summary>
    public Guid? PutawayLocationId { get; private set; }

    /// <summary>Putaway location code — redundant.</summary>
    public string? PutawayLocationCode { get; private set; }

    /// <summary>Expiry date — nullable, for expiry-managed materials.</summary>
    public DateTime? ExpiryDate { get; private set; }

    /// <summary>Production date — nullable.</summary>
    public DateTime? ProductionDate { get; private set; }

    /// <summary>Remark — optional note.</summary>
    public string? Remark { get; private set; }

    private InboundLine() { }

    public InboundLine(
        Guid id,
        Guid inboundOrderId,
        int lineNo,
        Guid materialId,
        string materialCode,
        string materialName,
        decimal planQuantity,
        string? batchNumber = null,
        DateTime? expiryDate = null,
        DateTime? productionDate = null,
        string? remark = null)
        : base(id)
    {
        InboundOrderId = inboundOrderId;
        LineNo = lineNo;
        MaterialId = materialId;
        MaterialCode = materialCode;
        MaterialName = materialName;
        PlanQuantity = planQuantity;
        ReceivedQuantity = 0m;
        BatchNumber = batchNumber;
        SerialNumberList = null;
        QualityStatus = QualityStatus.Pending;
        PutawayLocationId = null;
        PutawayLocationCode = null;
        ExpiryDate = expiryDate;
        ProductionDate = productionDate;
        Remark = remark;
    }

    /// <summary>
    /// Record received quantity — called during receipt confirmation.
    /// </summary>
    public void ReceiveQuantity(decimal receivedQuantity)
    {
        if (receivedQuantity < 0m)
        {
            throw new BusinessException("WMS:Inbound:InvalidReceivedQuantity",
                "Received quantity must be non-negative.");
        }

        ReceivedQuantity = receivedQuantity;
    }

    /// <summary>
    /// Set batch number — called during receipt confirmation or quality inspection.
    /// </summary>
    public void SetBatchNumber(string batchNumber)
    {
        BatchNumber = batchNumber;
    }

    /// <summary>
    /// Set quality status — called during quality inspection.
    /// Validates that the current status allows the transition.
    /// </summary>
    public void SetQualityStatus(QualityStatus status)
    {
        if (QualityStatus != QualityStatus.Pending && QualityStatus != QualityStatus.Skip)
        {
            // Allow re-inspection from Pending or Skip, but not from already Qualified/Unqualified
            if (QualityStatus == QualityStatus.Qualified && status != QualityStatus.Unqualified)
            {
                throw new BusinessException("WMS:Inbound:QualityStatusAlreadySet",
                    $"Quality status is already {QualityStatus.Name}. Cannot change to {status.Name}.");
            }
        }

        QualityStatus = status;
    }

    /// <summary>
    /// Set putaway location — called during putaway confirmation.
    /// </summary>
    public void SetPutawayLocation(Guid locationId, string locationCode)
    {
        if (QualityStatus == QualityStatus.Unqualified)
        {
            throw new BusinessException("WMS:Inbound:UnqualifiedPutaway",
                $"Cannot set putaway location for unqualified material {MaterialCode}. (IN-005)");
        }

        PutawayLocationId = locationId;
        PutawayLocationCode = locationCode;
    }

    /// <summary>
    /// Update expiry information — production date and expiry date.
    /// </summary>
    public void UpdateExpiryInfo(DateTime? expiryDate, DateTime? productionDate)
    {
        ExpiryDate = expiryDate;
        ProductionDate = productionDate;
    }

    /// <summary>
    /// Set serial number list — for serial-managed materials.
    /// </summary>
    public void SetSerialNumberList(List<string> serialNumbers)
    {
        SerialNumberList = serialNumbers;
    }
}
