using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inventory.Application.Services;

/// <summary>
/// Inventory Ledger App Service — read-only service for ledger entry queries.
/// No update/delete operations available (BR-010).
/// </summary>
public class InventoryLedgerAppService : ApplicationService, IInventoryLedgerAppService
{
    private readonly IInventoryLedgerRepository _ledgerRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;

    public InventoryLedgerAppService(
        IInventoryLedgerRepository ledgerRepository,
        IInventoryBalanceRepository balanceRepository)
    {
        _ledgerRepository = ledgerRepository;
        _balanceRepository = balanceRepository;
    }

    public async Task<PagedResultDto<InventoryLedgerOutputDto>> GetListAsync(InventoryLedgerQueryDto query)
    {
        var queryable = await _ledgerRepository.GetQueryableAsync();

        if (query.BalanceId.HasValue)
            queryable = queryable.Where(l => l.InventoryBalanceId == query.BalanceId.Value);
        if (!string.IsNullOrEmpty(query.SourceOrderType))
            queryable = queryable.Where(l => l.SourceOrderType == query.SourceOrderType);
        if (query.SourceOrderId.HasValue)
            queryable = queryable.Where(l => l.SourceOrderId == query.SourceOrderId.Value);
        if (query.StartTime.HasValue)
            queryable = queryable.Where(l => l.OperationTime >= query.StartTime.Value);
        if (query.EndTime.HasValue)
            queryable = queryable.Where(l => l.OperationTime <= query.EndTime.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(l => l.OperationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<InventoryLedgerOutputDto>(totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    public async Task<InventoryLedgerOutputDto> GetAsync(Guid id)
    {
        var entry = await _ledgerRepository.GetAsync(id);
        return MapToOutputDto(entry);
    }

    public async Task<List<InventoryLedgerOutputDto>> GetByBalanceIdAsync(Guid balanceId)
    {
        var entries = await _ledgerRepository.GetByBalanceIdAsync(balanceId);
        return entries.Select(MapToOutputDto).ToList();
    }

    public async Task<List<InventoryLedgerOutputDto>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
    {
        var entries = await _ledgerRepository.GetBySourceOrderAsync(sourceOrderType, sourceOrderId);
        return entries.Select(MapToOutputDto).ToList();
    }

    public async Task<List<InventoryLedgerOutputDto>> GetByMaterialTimeAsync(Guid materialId, DateTime? startTime = null, DateTime? endTime = null)
    {
        var entries = await _ledgerRepository.GetByMaterialAsync(materialId, startTime, endTime);
        return entries.Select(MapToOutputDto).ToList();
    }

    private InventoryLedgerOutputDto MapToOutputDto(InventoryLedgerEntry entry)
    {
        return new InventoryLedgerOutputDto
        {
            Id = entry.Id,
            InventoryBalanceId = entry.InventoryBalanceId,
            OperationTypeValue = entry.OperationType.Value,
            OperationTypeName = entry.OperationType.Description,
            OperationQuantity = entry.OperationQuantity,
            BeforeQuantity = entry.BeforeQuantity,
            AfterQuantity = entry.AfterQuantity,
            BeforeAvailable = entry.BeforeAvailable,
            AfterAvailable = entry.AfterAvailable,
            OperationTime = entry.OperationTime,
            OperatorId = entry.OperatorId,
            OperatorName = entry.OperatorName,
            SourceOrderType = entry.SourceOrderType,
            SourceOrderId = entry.SourceOrderId,
            SourceOrderNo = entry.SourceOrderNo,
            Remark = entry.Remark,
            CreationTime = entry.CreationTime
        };
    }
}
