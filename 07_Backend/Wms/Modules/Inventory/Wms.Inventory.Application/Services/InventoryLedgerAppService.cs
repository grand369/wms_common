using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Material.Domain.Repositories;
using Wms.Warehouse.Domain.Repositories;
using MaterialAgg = Wms.Material.Domain.Aggregates.Material;
using WarehouseAgg = Wms.Warehouse.Domain.Aggregates.Warehouse;
using Wms.Warehouse.Domain.Aggregates;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inventory.Application.Services;

/// <summary>
/// Inventory Ledger App Service — read-only service for ledger entry queries.
/// No update/delete operations available (BR-010).
/// Note: Cannot use LINQ JOIN across multiple DbContexts. Using batch query approach instead.
/// </summary>
public class InventoryLedgerAppService : ApplicationService, IInventoryLedgerAppService
{
    private readonly IInventoryLedgerRepository _ledgerRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;

    public InventoryLedgerAppService(
        IInventoryLedgerRepository ledgerRepository,
        IInventoryBalanceRepository balanceRepository,
        IMaterialRepository materialRepository,
        IWarehouseRepository warehouseRepository,
        ILocationRepository locationRepository)
    {
        _ledgerRepository = ledgerRepository;
        _balanceRepository = balanceRepository;
        _materialRepository = materialRepository;
        _warehouseRepository = warehouseRepository;
        _locationRepository = locationRepository;
    }

    public async Task<PagedResultDto<InventoryLedgerOutputDto>> GetListAsync(InventoryLedgerQueryDto query)
    {
        var ledgerQueryable = await _ledgerRepository.GetQueryableAsync();

        // Apply filters
        var filteredQuery = ledgerQueryable;
        if (query.BalanceId.HasValue)
            filteredQuery = filteredQuery.Where(l => l.InventoryBalanceId == query.BalanceId.Value);
        if (!string.IsNullOrEmpty(query.SourceOrderType))
            filteredQuery = filteredQuery.Where(l => l.SourceOrderType == query.SourceOrderType);
        if (query.SourceOrderId.HasValue)
            filteredQuery = filteredQuery.Where(l => l.SourceOrderId == query.SourceOrderId.Value);
        if (query.StartTime.HasValue)
            filteredQuery = filteredQuery.Where(l => l.OperationTime >= query.StartTime.Value);
        if (query.EndTime.HasValue)
            filteredQuery = filteredQuery.Where(l => l.OperationTime <= query.EndTime.Value);

        var totalCount = await AsyncExecuter.CountAsync(filteredQuery);
        var ledgerItems = await AsyncExecuter.ToListAsync(
            filteredQuery.OrderByDescending(l => l.OperationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        // Batch query related data from different contexts
        var result = await MapLedgerEntriesWithRelations(ledgerItems);

        return new PagedResultDto<InventoryLedgerOutputDto>(totalCount, result);
    }

    public async Task<InventoryLedgerOutputDto> GetAsync(Guid id)
    {
        var ledger = await _ledgerRepository.GetAsync(id);
        var result = await MapLedgerEntryWithRelations(ledger);
        return result;
    }

    public async Task<List<InventoryLedgerOutputDto>> GetByBalanceIdAsync(Guid balanceId)
    {
        var ledgerItems = await _ledgerRepository.GetByBalanceIdAsync(balanceId);
        var result = await MapLedgerEntriesWithRelations(ledgerItems);
        return result;
    }

    public async Task<List<InventoryLedgerOutputDto>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
    {
        var ledgerItems = await _ledgerRepository.GetBySourceOrderAsync(sourceOrderType, sourceOrderId);
        var result = await MapLedgerEntriesWithRelations(ledgerItems);
        return result;
    }

    public async Task<List<InventoryLedgerOutputDto>> GetByMaterialTimeAsync(Guid materialId, DateTime? startTime = null, DateTime? endTime = null)
    {
        var ledgerItems = await _ledgerRepository.GetByMaterialAsync(materialId, startTime, endTime);
        var result = await MapLedgerEntriesWithRelations(ledgerItems);
        return result;
    }

    /// <summary>
    /// Maps ledger entries with related data by batch querying from different contexts.
    /// </summary>
    private async Task<List<InventoryLedgerOutputDto>> MapLedgerEntriesWithRelations(List<InventoryLedgerEntry> ledgerItems)
    {
        if (!ledgerItems.Any()) return new List<InventoryLedgerOutputDto>();

        // Step 1: Get all balance IDs and query balances
        var balanceIds = ledgerItems.Select(l => l.InventoryBalanceId).Distinct().ToList();
        var balanceQueryable = await _balanceRepository.GetQueryableAsync();
        var balances = await AsyncExecuter.ToListAsync(balanceQueryable.Where(b => balanceIds.Contains(b.Id)));
        var balanceDict = balances.ToDictionary(b => b.Id);

        // Step 2: Get material info
        var materialIds = balances.Select(b => b.MaterialId).Distinct().ToList();
        var materialQueryable = await _materialRepository.GetQueryableAsync();
        var materials = await AsyncExecuter.ToListAsync(materialQueryable.Where(m => materialIds.Contains(m.Id)));
        var materialDict = materials.ToDictionary(m => m.Id);

        // Step 3: Get warehouse info
        var warehouseIds = balances.Select(b => b.WarehouseId).Distinct().ToList();
        var warehouseQueryable = await _warehouseRepository.GetQueryableAsync();
        var warehouses = await AsyncExecuter.ToListAsync(warehouseQueryable.Where(w => warehouseIds.Contains(w.Id)));
        var warehouseDict = warehouses.ToDictionary(w => w.Id);

        // Step 4: Get location info
        var locationIds = balances.Select(b => b.LocationId).Distinct().ToList();
        var locationQueryable = await _locationRepository.GetQueryableAsync();
        var locations = await AsyncExecuter.ToListAsync(locationQueryable.Where(l => locationIds.Contains(l.Id)));
        var locationDict = locations.ToDictionary(l => l.Id);

        // Step 5: Map to output DTOs
        return ledgerItems.Select(ledger => MapToOutputDto(ledger, balanceDict, materialDict, warehouseDict, locationDict)).ToList();
    }

    /// <summary>
    /// Maps a single ledger entry with related data.
    /// </summary>
    private async Task<InventoryLedgerOutputDto> MapLedgerEntryWithRelations(InventoryLedgerEntry ledger)
    {
        var balance = await _balanceRepository.GetAsync(ledger.InventoryBalanceId);
        var material = await _materialRepository.GetAsync(balance.MaterialId);
        var warehouse = await _warehouseRepository.GetAsync(balance.WarehouseId);
        var location = await _locationRepository.GetAsync(balance.LocationId);

        var balanceDict = new Dictionary<Guid, InventoryBalance> { { balance.Id, balance } };
        var materialDict = new Dictionary<Guid, MaterialAgg> { { material.Id, material } };
        var warehouseDict = new Dictionary<Guid, WarehouseAgg> { { warehouse.Id, warehouse } };
        var locationDict = new Dictionary<Guid, Location> { { location.Id, location } };

        return MapToOutputDto(ledger, balanceDict, materialDict, warehouseDict, locationDict);
    }

    /// <summary>
    /// Maps a ledger entry to output DTO with related data from dictionaries.
    /// </summary>
    private InventoryLedgerOutputDto MapToOutputDto(
        InventoryLedgerEntry ledger,
        Dictionary<Guid, InventoryBalance> balanceDict,
        Dictionary<Guid, MaterialAgg> materialDict,
        Dictionary<Guid, WarehouseAgg> warehouseDict,
        Dictionary<Guid, Location> locationDict)
    {
        balanceDict.TryGetValue(ledger.InventoryBalanceId, out var balance);
        materialDict.TryGetValue(balance?.MaterialId ?? Guid.Empty, out var material);
        warehouseDict.TryGetValue(balance?.WarehouseId ?? Guid.Empty, out var warehouse);
        locationDict.TryGetValue(balance?.LocationId ?? Guid.Empty, out var location);

        var inQuantity = ledger.OperationQuantity > 0 ? ledger.OperationQuantity : 0;
        var outQuantity = ledger.OperationQuantity < 0 ? Math.Abs(ledger.OperationQuantity) : 0;

        return new InventoryLedgerOutputDto
        {
            Id = ledger.Id,
            InventoryBalanceId = ledger.InventoryBalanceId,
            
            // Material info
            MaterialId = balance?.MaterialId ?? Guid.Empty,
            MaterialCode = balance?.MaterialCode ?? string.Empty,
            MaterialName = material?.MaterialName ?? string.Empty,
            
            // Warehouse info
            WarehouseId = balance?.WarehouseId ?? Guid.Empty,
            WarehouseCode = balance?.WarehouseCode ?? string.Empty,
            WarehouseName = warehouse?.WarehouseName ?? string.Empty,
            
            // Location info
            LocationId = balance?.LocationId ?? Guid.Empty,
            LocationCode = balance?.LocationCode ?? string.Empty,
            LocationName = balance?.LocationCode ?? string.Empty,
            
            // Operation info
            OperationTypeValue = ledger.OperationType.Value,
            OperationTypeName = ledger.OperationType.Description,
            OperationQuantity = ledger.OperationQuantity,
            InQuantity = inQuantity,
            OutQuantity = outQuantity,
            BalanceQuantity = ledger.AfterQuantity,
            BeforeQuantity = ledger.BeforeQuantity,
            AfterQuantity = ledger.AfterQuantity,
            BeforeAvailable = ledger.BeforeAvailable,
            AfterAvailable = ledger.AfterAvailable,
            OperationTime = ledger.OperationTime,
            OperatorId = ledger.OperatorId,
            OperatorName = ledger.OperatorName,
            
            // Source order info
            SourceOrderType = ledger.SourceOrderType,
            SourceOrderId = ledger.SourceOrderId,
            SourceOrderNo = ledger.SourceOrderNo,
            Remark = ledger.Remark,
            CreationTime = ledger.CreationTime
        };
    }
}
