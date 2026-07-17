using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Permissions;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Domain.Services;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inventory.Application.Services;

/// <summary>
/// Inventory Alert App Service — query, resolve, and scan alerts.
/// </summary>
public class InventoryAlertAppService : ApplicationService, IInventoryAlertAppService
{
    private readonly IInventoryAlertRepository _alertRepository;
    private readonly InventoryAlertService _alertService;

    public InventoryAlertAppService(
        IInventoryAlertRepository alertRepository,
        InventoryAlertService alertService)
    {
        _alertRepository = alertRepository;
        _alertService = alertService;
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
        await _alertService.ScanSafetyStockAsync();
        await _alertService.ScanExpiryAsync();
        await _alertService.ScanZeroInventoryAsync();
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
