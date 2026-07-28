using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Permissions;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Domain.Services;
using Wms.Material.Domain.Repositories;
using Wms.Warehouse.Domain.Repositories;
using MaterialAgg = Wms.Material.Domain.Aggregates.Material;
using WarehouseAgg = Wms.Warehouse.Domain.Aggregates.Warehouse;
using Wms.Warehouse.Domain.Aggregates;
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
    private readonly IMaterialRepository _materialRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IInventoryLedgerRepository _ledgerRepository;
    private readonly IInventoryFreezeOrderRepository _freezeOrderRepository;

    public InventoryBalanceAppService(
        IInventoryBalanceRepository balanceRepository,
        InventoryDomainService domainService,
        IMaterialRepository materialRepository,
        IWarehouseRepository warehouseRepository,
        ILocationRepository locationRepository,
        IInventoryLedgerRepository ledgerRepository,
        IInventoryFreezeOrderRepository freezeOrderRepository)
    {
        _balanceRepository = balanceRepository;
        _domainService = domainService;
        _materialRepository = materialRepository;
        _warehouseRepository = warehouseRepository;
        _locationRepository = locationRepository;
        _ledgerRepository = ledgerRepository;
        _freezeOrderRepository = freezeOrderRepository;
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

        var result = await MapBalancesWithRelations(items);

        return new PagedResultDto<InventoryBalanceOutputDto>(totalCount, result);
    }

    public async Task<InventoryBalanceOutputDto> GetAsync(Guid id)
    {
        var balance = await _balanceRepository.GetAsync(id);
        var result = await MapBalanceWithRelations(balance);
        return result;
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

        var result = await MapBalancesWithRelations(items);
        return result;
    }

    public async Task<List<InventoryBalanceOutputDto>> GetByMaterialAsync(Guid materialId)
    {
        var balances = await _balanceRepository.GetByMaterialAsync(materialId);
        var result = await MapBalancesWithRelations(balances);
        return result;
    }

    public async Task<List<InventoryBalanceOutputDto>> GetByLocationAsync(Guid locationId)
    {
        var balances = await _balanceRepository.GetByLocationAsync(locationId);
        var result = await MapBalancesWithRelations(balances);
        return result;
    }

    public async Task<List<InventoryBalanceOutputDto>> GetByWarehouseAsync(Guid warehouseId)
    {
        var balances = await _balanceRepository.GetByWarehouseAsync(warehouseId);
        var result = await MapBalancesWithRelations(balances);
        return result;
    }

    public async Task<List<InventoryBalanceOutputDto>> GetByBatchAsync(string batchNumber)
    {
        var balances = await _balanceRepository.GetByBatchAsync(batchNumber);
        var result = await MapBalancesWithRelations(balances);
        return result;
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
        var resultDto = await MapBalanceWithRelations(balance);
        return resultDto;
    }

    [Authorize(WmsInventoryPermissions.Balance.Snapshot)]
    public async Task SnapshotAsync()
    {
        // v1.0 placeholder — snapshot will be implemented in v1.1 with dedicated snapshot table
        Logger.LogInformation("Inventory snapshot requested (v1.0 placeholder)");
    }

    [Authorize(WmsInventoryPermissions.Balance.Freeze)]
    public async Task<InventoryBalanceOutputDto> FreezeAsync(Guid id, InventoryBalanceFreezeDto dto)
    {
        var balance = await _balanceRepository.GetAsync(id);
        
        // Save current values before freeze
        var beforeQuantity = balance.Quantity;
        var beforeAvailable = balance.AvailableQuantity;
        
        balance.FreezeQuantity(dto.Qty, "ManualFreeze", Guid.Empty);
        await _balanceRepository.UpdateAsync(balance);
        
        // Create a simplified freeze order record for business tracking
        var freezeOrder = new InventoryFreezeOrder(
            GuidGenerator.Create(),
            GenerateFreezeOrderNo(),
            FreezeScope.ByBatch, // Default to ByBatch for single balance freeze
            dto.Reason,
            balance.WarehouseId,
            balance.WarehouseCode,
            balance.MaterialId,
            balance.MaterialCode,
            dto.Qty,
            DateTime.UtcNow);
        freezeOrder.Approve(); // Auto-approve for manual freeze
        await _freezeOrderRepository.InsertAsync(freezeOrder);
        
        // Record ledger entry with freeze order reference
        var ledgerEntry = new InventoryLedgerEntry(
            GuidGenerator.Create(),
            balance.Id,
            InventoryOperationType.Freeze,
            dto.Qty,
            beforeQuantity,
            balance.Quantity,
            beforeAvailable,
            balance.AvailableQuantity,
            DateTime.UtcNow,
            CurrentUser.Id ?? Guid.Empty,
            CurrentUser.Name ?? string.Empty,
            "InventoryFreezeOrder",
            freezeOrder.Id,
            freezeOrder.FreezeOrderNo,
            dto.Reason);
        await _ledgerRepository.InsertAsync(ledgerEntry);

        var result = await MapBalanceWithRelations(balance);
        return result;
    }

    [Authorize(WmsInventoryPermissions.Balance.Freeze)]
    public async Task<InventoryBalanceOutputDto> UnfreezeAsync(Guid id)
    {
        var balance = await _balanceRepository.GetAsync(id);
        var frozenQty = balance.FrozenQuantity;
        if (frozenQty <= 0)
        {
            throw new BusinessException("WMS:Inventory:NoFrozenQuantity",
                "No frozen quantity to unfreeze.");
        }
        
        // Save current values before unfreeze
        var beforeQuantity = balance.Quantity;
        var beforeAvailable = balance.AvailableQuantity;
        
        balance.UnfreezeQuantity(frozenQty, "ManualUnfreeze", Guid.Empty);
        await _balanceRepository.UpdateAsync(balance);
        
        // Record ledger entry
        var ledgerEntry = new InventoryLedgerEntry(
            GuidGenerator.Create(),
            balance.Id,
            InventoryOperationType.Unfreeze,
            frozenQty,
            beforeQuantity,
            balance.Quantity,
            beforeAvailable,
            balance.AvailableQuantity,
            DateTime.UtcNow,
            CurrentUser.Id ?? Guid.Empty,
            CurrentUser.Name ?? string.Empty,
            "ManualUnfreeze",
            Guid.Empty,
            string.Empty,
            "Manual unfreeze");
        await _ledgerRepository.InsertAsync(ledgerEntry);

        var result = await MapBalanceWithRelations(balance);
        return result;
    }

    /// <summary>
    /// Maps balance entries with related data by batch querying from different contexts.
    /// </summary>
    private async Task<List<InventoryBalanceOutputDto>> MapBalancesWithRelations(List<InventoryBalance> balances)
    {
        if (!balances.Any()) return new List<InventoryBalanceOutputDto>();

        // Step 1: Get material info
        var materialIds = balances.Select(b => b.MaterialId).Distinct().ToList();
        var materialQueryable = await _materialRepository.GetQueryableAsync();
        var materials = await AsyncExecuter.ToListAsync(materialQueryable.Where(m => materialIds.Contains(m.Id)));
        var materialDict = materials.ToDictionary(m => m.Id);

        // Step 2: Get warehouse info
        var warehouseIds = balances.Select(b => b.WarehouseId).Distinct().ToList();
        var warehouseQueryable = await _warehouseRepository.GetQueryableAsync();
        var warehouses = await AsyncExecuter.ToListAsync(warehouseQueryable.Where(w => warehouseIds.Contains(w.Id)));
        var warehouseDict = warehouses.ToDictionary(w => w.Id);

        // Step 3: Get location info
        var locationIds = balances.Select(b => b.LocationId).Distinct().ToList();
        var locationQueryable = await _locationRepository.GetQueryableAsync();
        var locations = await AsyncExecuter.ToListAsync(locationQueryable.Where(l => locationIds.Contains(l.Id)));
        var locationDict = locations.ToDictionary(l => l.Id);

        // Step 4: Map to output DTOs
        return balances.Select(balance => MapToOutputDto(balance, materialDict, warehouseDict, locationDict)).ToList();
    }

    /// <summary>
    /// Maps a single balance with related data.
    /// </summary>
    private async Task<InventoryBalanceOutputDto> MapBalanceWithRelations(InventoryBalance balance)
    {
        var material = await _materialRepository.GetAsync(balance.MaterialId);
        var warehouse = await _warehouseRepository.GetAsync(balance.WarehouseId);
        var location = await _locationRepository.GetAsync(balance.LocationId);

        var materialDict = new Dictionary<Guid, MaterialAgg> { { material.Id, material } };
        var warehouseDict = new Dictionary<Guid, WarehouseAgg> { { warehouse.Id, warehouse } };
        var locationDict = new Dictionary<Guid, Location> { { location.Id, location } };

        return MapToOutputDto(balance, materialDict, warehouseDict, locationDict);
    }

    private InventoryBalanceOutputDto MapToOutputDto(
        InventoryBalance balance,
        Dictionary<Guid, MaterialAgg> materialDict,
        Dictionary<Guid, WarehouseAgg> warehouseDict,
        Dictionary<Guid, Location> locationDict)
    {
        materialDict.TryGetValue(balance.MaterialId, out var material);
        warehouseDict.TryGetValue(balance.WarehouseId, out var warehouse);
        locationDict.TryGetValue(balance.LocationId, out var location);

        return new InventoryBalanceOutputDto
        {
            Id = balance.Id,
            MaterialId = balance.MaterialId,
            MaterialCode = balance.MaterialCode,
            MaterialName = material?.MaterialName ?? string.Empty,
            WarehouseId = balance.WarehouseId,
            WarehouseCode = balance.WarehouseCode,
            WarehouseName = warehouse?.WarehouseName ?? string.Empty,
            LocationId = balance.LocationId,
            LocationCode = balance.LocationCode,
            LocationName = balance.LocationCode ?? string.Empty,
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

    private string GenerateFreezeOrderNo()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var random = new Random();
        var suffix = random.Next(1000, 9999).ToString();
        return $"FRZ-{timestamp}-{suffix}";
    }
}
