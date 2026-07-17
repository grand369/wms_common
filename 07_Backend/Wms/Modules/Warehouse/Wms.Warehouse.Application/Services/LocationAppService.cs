using Volo.Abp.Application.Services;
using Wms.Warehouse.Application.Contracts.Dtos;
using Wms.Warehouse.Application.Contracts.Permissions;
using Wms.Warehouse.Application.Contracts.Services;
using Wms.Warehouse.Domain.Aggregates;
using Wms.Warehouse.Domain.Enums;
using Wms.Warehouse.Domain.Repositories;
using Wms.Warehouse.Domain.Services;

namespace Wms.Warehouse.Application.Services;

/// <summary>
/// Location App Service — implements ILocationAppService.
/// (Phase 6 API Design)
/// </summary>
public class LocationAppService : ApplicationService, ILocationAppService
{
    private readonly ILocationRepository _locationRepository;
    private readonly WarehouseManager _warehouseManager;

    public LocationAppService(
        ILocationRepository locationRepository,
        WarehouseManager warehouseManager)
    {
        _locationRepository = locationRepository;
        _warehouseManager = warehouseManager;
    }

    public async Task<LocationOutputDto> GetAsync(Guid id)
    {
        var location = await _locationRepository.GetAsync(id);
        return MapToOutputDto(location);
    }

    public async Task<PagedResultDto<LocationOutputDto>> GetListAsync(LocationQueryDto query)
    {
        var queryable = await _locationRepository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(query.LocationCode),
                l => l.LocationCode.Contains(query.LocationCode!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.WarehouseId),
                l => l.WarehouseId == query.WarehouseId)
            .WhereIf(!string.IsNullOrWhiteSpace(query.AreaId),
                l => l.AreaId == query.AreaId)
            .WhereIf(query.LocationType.HasValue,
                l => l.LocationType == query.LocationType.Value)
            .WhereIf(query.StorageCondition.HasValue,
                l => l.StorageCondition == query.StorageCondition.Value)
            .WhereIf(query.IsActive.HasValue,
                l => l.IsActive == query.IsActive.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(query.BarcodeId),
                l => l.BarcodeId == query.BarcodeId);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(l => l.CreationTime)
                     .PageBy(query.PageIndex, query.PageSize));

        return new PagedResultDto<LocationOutputDto>(
            totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    public async Task<List<LocationOutputDto>> GetListByWarehouseIdAsync(string warehouseId)
    {
        var locations = await _locationRepository.GetListByWarehouseIdAsync(warehouseId);
        return locations.Select(MapToOutputDto).ToList();
    }

    public async Task<List<LocationOutputDto>> GetListByAreaIdAsync(string areaId)
    {
        var locations = await _locationRepository.GetListByAreaIdAsync(areaId);
        return locations.Select(MapToOutputDto).ToList();
    }

    public async Task<List<LocationOutputDto>> GetAvailableLocationsAsync(string warehouseId, int? storageCondition = null)
    {
        var locations = await _locationRepository.GetAvailableLocationsAsync(warehouseId, storageCondition);
        return locations.Select(MapToOutputDto).ToList();
    }

    public async Task<LocationOutputDto> GetByBarcodeAsync(string barcodeId)
    {
        var location = await _locationRepository.FindByBarcodeIdAsync(barcodeId);
        if (location == null)
            throw new Volo.Abp.BusinessException("WMS:Warehouse:LocationNotFound").WithData("BarcodeId", barcodeId);
        return MapToOutputDto(location);
    }

    [Authorize(WmsWarehousePermissions.Locations.Create)]
    public async Task<LocationOutputDto> CreateAsync(LocationCreateDto input)
    {
        await _warehouseManager.ValidateLocationCodeUniqueAsync(input.LocationCode);

        var location = new Location(
            GuidGenerator.Create(),
            input.LocationCode,
            input.WarehouseId,
            input.WarehouseCode,
            input.AreaId,
            input.AreaCode,
            input.BarcodeId,
            input.LocationType,
            input.StorageCondition,
            input.MaxWeight,
            input.MaxCapacity,
            input.Row,
            input.Column,
            input.Layer,
            input.IsActive);

        await _locationRepository.InsertAsync(location);
        return MapToOutputDto(location);
    }

    [Authorize(WmsWarehousePermissions.Locations.Update)]
    public async Task<LocationOutputDto> UpdateAsync(Guid id, LocationUpdateDto input)
    {
        var location = await _locationRepository.GetAsync(id);

        location.SetLocationType(input.LocationType);
        location.SetStorageCondition(input.StorageCondition);
        location.SetMaxWeight(input.MaxWeight);
        location.SetMaxCapacity(input.MaxCapacity);
        location.SetCoordinates(input.Row, input.Column, input.Layer);

        if (input.IsActive && !location.IsActive)
            location.SetActive();
        else if (!input.IsActive && location.IsActive)
            location.Deactivate();

        await _locationRepository.UpdateAsync(location);
        return MapToOutputDto(location);
    }

    [Authorize(WmsWarehousePermissions.Locations.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _locationRepository.DeleteAsync(id);
    }

    [Authorize(WmsWarehousePermissions.Locations.Activate)]
    public async Task ActivateAsync(Guid id)
    {
        var location = await _locationRepository.GetAsync(id);
        location.SetActive();
        await _locationRepository.UpdateAsync(location);
    }

    [Authorize(WmsWarehousePermissions.Locations.Deactivate)]
    public async Task DeactivateAsync(Guid id)
    {
        var location = await _locationRepository.GetAsync(id);
        location.Deactivate();
        await _locationRepository.UpdateAsync(location);
    }

    private LocationOutputDto MapToOutputDto(Location location)
    {
        var typeEnum = LocationType.FromValue(location.LocationType);
        var condEnum = StorageConditionType.FromValue(location.StorageCondition);

        return new LocationOutputDto
        {
            Id = location.Id,
            LocationCode = location.LocationCode,
            WarehouseId = location.WarehouseId,
            WarehouseCode = location.WarehouseCode,
            AreaId = location.AreaId,
            AreaCode = location.AreaCode,
            LocationType = location.LocationType,
            LocationTypeDescription = typeEnum.Description,
            MaxWeight = location.MaxWeight,
            MaxCapacity = location.MaxCapacity,
            CurrentWeight = location.CurrentWeight,
            CurrentCapacity = location.CurrentCapacity,
            StorageCondition = location.StorageCondition,
            StorageConditionDescription = condEnum.Description,
            BarcodeId = location.BarcodeId,
            Row = location.Row,
            Column = location.Column,
            Layer = location.Layer,
            IsActive = location.IsActive,
            CreationTime = location.CreationTime,
            CreatorId = location.CreatorId
        };
    }
}
