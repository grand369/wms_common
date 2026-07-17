namespace Wms.Shared.Domain.Interfaces;

/// <summary>
/// Bonded Warehouse Service Interface — v2.0 extension point (ADR-005).
/// Provides customs compliance and bonded warehouse operations.
/// v1.0 scope exclusion: bonded warehouse / hazardous chemicals compliance.
/// </summary>
public interface IBondedWarehouseService
{
    /// <summary>
    /// Verifies customs declaration status for bonded goods.
    /// </summary>
    Task<bool> VerifyCustomsDeclarationAsync(Guid materialId, string batchNumber);

    /// <summary>
    /// Checks bonded warehouse quota availability.
    /// </summary>
    Task<bool> CheckQuotaAvailabilityAsync(Guid warehouseId, decimal quantity);
}
