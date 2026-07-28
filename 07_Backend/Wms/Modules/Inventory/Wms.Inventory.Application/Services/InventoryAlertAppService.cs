using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Permissions;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Domain.Services;
using Wms.Material.Domain.Repositories;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inventory.Application.Services;

/// <summary>
/// Inventory Alert App Service — query, resolve, and scan alerts.
/// Safety stock alert logic queries Material module to get SafetyStockQuantity.
/// </summary>
public class InventoryAlertAppService : ApplicationService, IInventoryAlertAppService
{
    private readonly IInventoryAlertRepository _alertRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly InventoryAlertService _alertService;
    private readonly IMaterialRepository _materialRepository;

    public InventoryAlertAppService(
        IInventoryAlertRepository alertRepository,
        IInventoryBalanceRepository balanceRepository,
        InventoryAlertService alertService,
        IMaterialRepository materialRepository)
    {
        _alertRepository = alertRepository;
        _balanceRepository = balanceRepository;
        _alertService = alertService;
        _materialRepository = materialRepository;
    }

    public async Task<PagedResultDto<InventoryAlertOutputDto>> GetListAsync(InventoryAlertQueryDto query)
    {
        var queryable = await _alertRepository.GetQueryableAsync();
        if (query.AlertTypeValue.HasValue)
            queryable = queryable.Where(a => a.AlertType.Value == query.AlertTypeValue.Value);
        if (query.MaterialId.HasValue)
            queryable = queryable.Where(a => a.MaterialId == query.MaterialId.Value);
        if (query.IsResolved.HasValue)
            queryable = queryable.Where(a => a.IsResolved == query.IsResolved.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(a => a.AlertTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<InventoryAlertOutputDto>(totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    public async Task<List<InventoryAlertOutputDto>> GetActiveAsync()
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync();
        return alerts.Select(MapToOutputDto).ToList();
    }

    [Authorize(WmsInventoryPermissions.Alert.Resolve)]
    public async Task<InventoryAlertOutputDto> ResolveAsync(Guid id)
    {
        var alert = await _alertRepository.GetAsync(id);
        alert.Resolve();
        await _alertRepository.UpdateAsync(alert);
        return MapToOutputDto(alert);
    }

    [Authorize(WmsInventoryPermissions.Alert.Scan)]
    public async Task ScanAsync()
    {
        // Safety stock alert — use application layer logic with Material module query
        await ScanSafetyStockWithMaterialInfoAsync();

        // Other alerts — use domain service
        await _alertService.ScanExpiryAsync();
        await _alertService.ScanZeroInventoryAsync();
    }

    /// <summary>
    /// Safety stock alert with Material module integration.
    /// Queries Material module to get SafetyStockQuantity for each material.
    /// </summary>
    private async Task ScanSafetyStockWithMaterialInfoAsync()
    {
        // Get all inventory balances
        var balanceQueryable = await _balanceRepository.GetQueryableAsync();
        var balances = await AsyncExecuter.ToListAsync(
            balanceQueryable.Where(b => !b.IsDeleted));

        if (!balances.Any()) return;

        // Get distinct material IDs
        var materialIds = balances.Select(b => b.MaterialId).Distinct().ToList();

        // Query materials to get safety stock quantities
        var materialQueryable = await _materialRepository.GetQueryableAsync();
        var materials = await AsyncExecuter.ToListAsync(
            materialQueryable.Where(m => materialIds.Contains(m.Id)));

        var materialDict = materials.ToDictionary(m => m.Id);

        // Check each balance against its safety stock
        foreach (var balance in balances)
        {
            if (!materialDict.TryGetValue(balance.MaterialId, out var material))
                continue;

            var safetyStockQuantity = material.InventoryAttribute?.SafetyStockQuantity ?? 0m;

            // Skip if safety stock is not configured (0 means no alert)
            if (safetyStockQuantity <= 0)
                continue;

            // Check if available quantity is below safety stock
            if (balance.AvailableQuantity <= safetyStockQuantity)
            {
                // Create alert with actual safety stock threshold
                var alert = new InventoryAlert(
                    GuidGenerator.Create(),
                    AlertType.SafetyStock,
                    balance.MaterialId,
                    balance.MaterialCode,
                    balance.WarehouseId,
                    balance.WarehouseCode,
                    balance.AvailableQuantity,
                    safetyStockQuantity);

                await _alertRepository.InsertAsync(alert);

                // Update balance's SafetyStockQuantity field (sync from material)
                if (balance.SafetyStockQuantity != safetyStockQuantity)
                {
                    balance.UpdateSafetyStockQuantity(safetyStockQuantity);
                    await _balanceRepository.UpdateAsync(balance);
                }
            }
        }
    }

    private InventoryAlertOutputDto MapToOutputDto(InventoryAlert alert)
    {
        return new InventoryAlertOutputDto
        {
            Id = alert.Id,
            AlertTypeValue = alert.AlertType.Value,
            AlertTypeName = alert.AlertType.Description,
            MaterialId = alert.MaterialId,
            MaterialCode = alert.MaterialCode,
            WarehouseId = alert.WarehouseId,
            WarehouseCode = alert.WarehouseCode,
            CurrentQuantity = alert.CurrentQuantity,
            ThresholdQuantity = alert.ThresholdQuantity,
            IsResolved = alert.IsResolved,
            AlertTime = alert.AlertTime,
            ResolveTime = alert.ResolveTime,
            CreationTime = alert.CreationTime
        };
    }
}