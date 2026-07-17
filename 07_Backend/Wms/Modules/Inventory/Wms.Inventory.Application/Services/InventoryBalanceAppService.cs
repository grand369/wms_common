using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Permissions;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Domain.Services;
using Wms.Shared.Domain.Enums;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inventory.Application.Services;

/// <summary>
/// Inventory Balance App Service — core application service for inventory queries and initialization.
/// Injects IInventoryBalanceRepository + InventoryDomainService.
/// </summary>
public class InventoryBalanceAppService : ApplicationService, IInventoryBalanceAppService
{
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly InventoryDomainService _domainService;

    public InventoryBalanceAppService(
        IInventoryBalanceRepository balanceRepository,
        InventoryDomainService domainService)
    {
        _balanceRepository = balanceRepository;
        _domainService = domainService;
    }

    public async Task<PagedResultDto<InventoryBalanceOutputDto>> GetListAsync(InventoryBalanceQueryDto query)
    {
        var queryable = await _balanceRepository.GetQueryableAsync();

        if (query.MaterialId.HasValue)
            queryable = queryable.Where(b => b.MaterialId == query.MaterialId.Value);
        if (query.WarehouseId.HasValue)
            queryable = queryable.Where(b => b.WarehouseId == query.WarehouseId.Value);
        if (query.LocationId.HasValue)
            queryable = queryable.Where(b => b.LocationId == query.LocationId.Value);
        if (!string.IsNullOrEmpty(query.BatchNumber))
            queryable = queryable.Where(b => b.BatchNumber == query.BatchNumber);
        if (query.InventoryStatusValue.HasValue)
            queryable = queryable.Where(b => b.InventoryStatus.Value == query.InventoryStatusValue.Value);
        if (!string.IsNullOrEmpty(query.Keyword))
            queryable = queryable.Where(b => b.MaterialCode.Contains(query.Keyword) ||
                b.WarehouseCode.Contains(query.Keyword) || b.LocationCode.Contains(query.Keyword));

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(b => b.LastOperationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<InventoryBalanceOutputDto>(totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    public async Task<InventoryBalanceOutputDto> GetAsync(Guid id)
    {
        var balance = await _balanceRepository.GetAsync(id);
        return MapToOutputDto(balance);
    }

    public async Task<List<InventoryBalanceOutputDto>> GetAvailableAsync(InventoryBalanceAvailableQueryDto query)
    {
        var queryable = await _balanceRepository.GetQueryableAsync();
        queryable = queryable.Where(b => b.AvailableQuantity > 0);

        if (query.MaterialId.HasValue)
            queryable = queryable.Where(b => b.MaterialId == query.MaterialId.Value);
        if (query.WarehouseId.HasValue)
            queryable = queryable.Where(b => b.WarehouseId == query.WarehouseId.Value);
        if (query.LocationId.HasValue)
            queryable = queryable.Where(b => b.LocationId == query.LocationId.Value);

        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(b => b.AvailableQuantity));

        return items.Select(MapToOutputDto).ToList();
    }

    public async Task<List<InventoryBalanceOutputDto>> GetByMaterialAsync(Guid materialId)
    {
        var balances = await _balanceRepository.GetByMaterialAsync(materialId);
        return balances.Select(MapToOutputDto).ToList();
    }

    public async Task<List<InventoryBalanceOutputDto>> GetByLocationAsync(Guid locationId)
    {
        var balances = await _balanceRepository.GetByLocationAsync(locationId);
        return balances.Select(MapToOutputDto).ToList();
    }

    public async Task<List<InventoryBalanceOutputDto>> GetByWarehouseAsync(Guid warehouseId)
    {
        var balances = await _balanceRepository.GetByWarehouseAsync(warehouseId);
        return balances.Select(MapToOutputDto).ToList();
    }

    public async Task<List<InventoryBalanceOutputDto>> GetByBatchAsync(string batchNumber)
    {
        var balances = await _balanceRepository.GetByBatchAsync(batchNumber);
        return balances.Select(MapToOutputDto).ToList();
    }

    public async Task<InventorySummaryDto> GetSummaryAsync()
    {
        var queryable = await _balanceRepository.GetQueryableAsync();
        var balances = await AsyncExecuter.ToListAsync(queryable);

        return new InventorySummaryDto
        {
            TotalBalanceCount = balances.Count,
            TotalQuantity = balances.Sum(b => b.Quantity),
            TotalAvailableQuantity = balances.Sum(b => b.AvailableQuantity),
            TotalReservedQuantity = balances.Sum(b => b.ReservedQuantity),
            TotalFrozenQuantity = balances.Sum(b => b.FrozenQuantity),
            TotalInTransitQuantity = balances.Sum(b => b.InTransitQuantity),
            MaterialCount = balances.Select(b => b.MaterialId).Distinct().Count(),
            ZeroInventoryCount = balances.Count(b => b.Quantity == 0),
            NearExpiryCount = balances.Count(b => b.ExpiryDate.HasValue &&
                (b.ExpiryDate.Value - DateTime.UtcNow).Days <= 30),
            BelowSafetyStockCount = balances.Count(b => b.AvailableQuantity <= 0)
        };
    }

    [Authorize(WmsInventoryPermissions.Balance.Initialize)]
    public async Task<InventoryBalanceOutputDto> InitializeAsync(InventoryBalanceInitializeDto dto)
    {
        var result = await _domainService.IncreaseInventoryAsync(
            dto.MaterialId, dto.WarehouseId, dto.LocationId, dto.BatchNumber,
            dto.Quantity, dto.MaterialCode, dto.WarehouseCode, dto.LocationCode,
            dto.SourceOrderType, dto.SourceOrderId, dto.AllowNegativeInventory);

        var balance = await _balanceRepository.GetAsync(result.BalanceId);
        return MapToOutputDto(balance);
    }

    [Authorize(WmsInventoryPermissions.Balance.Snapshot)]
    public async Task SnapshotAsync()
    {
        // v1.0 placeholder — snapshot will be implemented in v1.1 with dedicated snapshot table
        Logger.LogInformation("Inventory snapshot requested (v1.0 placeholder)");
    }

    private InventoryBalanceOutputDto MapToOutputDto(InventoryBalance balance)
    {
        return new InventoryBalanceOutputDto
        {
            Id = balance.Id,
            MaterialId = balance.MaterialId,
            MaterialCode = balance.MaterialCode,
            WarehouseId = balance.WarehouseId,
            WarehouseCode = balance.WarehouseCode,
            LocationId = balance.LocationId,
            LocationCode = balance.LocationCode,
            BatchNumber = balance.BatchNumber,
            InventoryStatusValue = balance.InventoryStatus.Value,
            InventoryStatusName = balance.InventoryStatus.Description,
            Quantity = balance.Quantity,
            ReservedQuantity = balance.ReservedQuantity,
            FrozenQuantity = balance.FrozenQuantity,
            InTransitQuantity = balance.InTransitQuantity,
            AvailableQuantity = balance.AvailableQuantity,
            ExpiryDate = balance.ExpiryDate,
            ProductionDate = balance.ProductionDate,
            SupplierId = balance.SupplierId,
            SupplierName = balance.SupplierName,
            UnitCost = balance.UnitCost,
            LastOperationTime = balance.LastOperationTime,
            ConcurrencyVersion = balance.ConcurrencyVersion,
            CreationTime = balance.CreationTime
        };
    }
}
