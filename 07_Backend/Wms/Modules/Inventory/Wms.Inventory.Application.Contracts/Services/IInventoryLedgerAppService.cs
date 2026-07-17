using Wms.Inventory.Application.Contracts.Dtos;

namespace Wms.Inventory.Application.Contracts.Services;

/// <summary>
/// Inventory Ledger App Service Interface — read-only service for ledger queries.
/// </summary>
public interface IInventoryLedgerAppService
{
    Task<PagedResultDto<InventoryLedgerOutputDto>> GetListAsync(InventoryLedgerQueryDto query);
    Task<InventoryLedgerOutputDto> GetAsync(Guid id);
    Task<List<InventoryLedgerOutputDto>> GetByBalanceIdAsync(Guid balanceId);
    Task<List<InventoryLedgerOutputDto>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId);
    Task<List<InventoryLedgerOutputDto>> GetByMaterialTimeAsync(Guid materialId, DateTime? startTime = null, DateTime? endTime = null);
}
