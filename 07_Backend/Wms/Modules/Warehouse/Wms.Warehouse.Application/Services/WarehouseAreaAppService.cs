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
/// Warehouse Area App Service — implements IWarehouseAreaAppService.
/// (Phase 6 API Design)
/// </summary>
public class WarehouseAreaAppService : ApplicationService, IWarehouseAreaAppService
{
    private readonly IWarehouseAreaRepository _areaRepository;
    private readonly WarehouseManager _warehouseManager;

    public WarehouseAreaAppService(
        IWarehouseAreaRepository areaRepository,
        WarehouseManager warehouseManager)
    {
        _areaRepository = areaRepository;
        _warehouseManager = warehouseManager;
    }

    public async Task<WarehouseAreaOutputDto> GetAsync(Guid id)
    {
        var area = await _areaRepository.GetAsync(id);
        return MapToOutputDto(area);
    }

    public async Task<PagedResultDto<WarehouseAreaOutputDto>> GetListAsync(WarehouseAreaQueryDto query)
    {
        var queryable = await _areaRepository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(query.WarehouseId),
                a => a.WarehouseId == query.WarehouseId)
            .WhereIf(!string.IsNullOrWhiteSpace(query.AreaCode),
                a => a.AreaCode.Contains(query.AreaCode!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.AreaName),
                a => a.AreaName.Contains(query.AreaName!))
            .WhereIf(query.AreaFunction.HasValue,
                a => a.AreaFunction == query.AreaFunction.Value)
            .WhereIf(query.StorageEnvironment.HasValue,
                a => a.StorageEnvironment == query.StorageEnvironment.Value)
            .WhereIf(query.IsActive.HasValue,
                a => a.IsActive == query.IsActive.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(a => a.CreationTime)
                     .PageBy(query.PageIndex, query.PageSize));

        return new PagedResultDto<WarehouseAreaOutputDto>(
            totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    public async Task<List<WarehouseAreaOutputDto>> GetListByWarehouseIdAsync(string warehouseId)
    {
        var areas = await _areaRepository.GetListByWarehouseIdAsync(warehouseId);
        return areas.Select(MapToOutputDto).ToList();
    }

    [Authorize(WmsWarehousePermissions.Areas.Create)]
    public async Task<WarehouseAreaOutputDto> CreateAsync(WarehouseAreaCreateDto input)
    {
        await _warehouseManager.ValidateAreaCodeUniqueAsync(input.AreaCode, input.WarehouseId);

        var area = new WarehouseArea(
            GuidGenerator.Create(),
            input.AreaCode,
            input.AreaName,
            input.WarehouseId,
            input.WarehouseCode,
            input.AreaFunction,
            input.StorageEnvironment,
            input.MaxCapacity,
            input.CurrentCapacity,
            input.IsActive);

        await _areaRepository.InsertAsync(area);
        return MapToOutputDto(area);
    }

    [Authorize(WmsWarehousePermissions.Areas.Update)]
    public async Task<WarehouseAreaOutputDto> UpdateAsync(Guid id, WarehouseAreaUpdateDto input)
    {
        var area = await _areaRepository.GetAsync(id);

        area.SetAreaName(input.AreaName);
        area.SetAreaFunction(input.AreaFunction);
        area.SetStorageEnvironment(input.StorageEnvironment);
        area.UpdateCapacity(input.MaxCapacity, input.CurrentCapacity);

        if (input.IsActive && !area.IsActive)
            area.SetActive();
        else if (!input.IsActive && area.IsActive)
            area.Deactivate();

        await _areaRepository.UpdateAsync(area);
        return MapToOutputDto(area);
    }

    [Authorize(WmsWarehousePermissions.Areas.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _areaRepository.DeleteAsync(id);
    }

    [Authorize(WmsWarehousePermissions.Areas.Update)]
    public async Task ActivateAsync(Guid id)
    {
        var area = await _areaRepository.GetAsync(id);
        area.SetActive();
        await _areaRepository.UpdateAsync(area);
    }

    [Authorize(WmsWarehousePermissions.Areas.Update)]
    public async Task DeactivateAsync(Guid id)
    {
        var area = await _areaRepository.GetAsync(id);
        area.Deactivate();
        await _areaRepository.UpdateAsync(area);
    }

    private WarehouseAreaOutputDto MapToOutputDto(WarehouseArea area)
    {
        var funcEnum = AreaFunction.FromValue(area.AreaFunction);
        var envEnum = StorageEnvironment.FromValue(area.StorageEnvironment);

        decimal? utilization = null;
        if (area.MaxCapacity != null && area.MaxCapacity != 0 && area.CurrentCapacity != null)
        {
            utilization = Math.Round(area.CurrentCapacity.Value / area.MaxCapacity.Value * 100, 2);
        }

        return new WarehouseAreaOutputDto
        {
            Id = area.Id,
            AreaCode = area.AreaCode,
            AreaName = area.AreaName,
            WarehouseId = area.WarehouseId,
            WarehouseCode = area.WarehouseCode,
            AreaFunction = area.AreaFunction,
            AreaFunctionDescription = funcEnum.Description,
            StorageEnvironment = area.StorageEnvironment,
            StorageEnvironmentDescription = envEnum.Description,
            MaxCapacity = area.MaxCapacity,
            CurrentCapacity = area.CurrentCapacity,
            UtilizationPercentage = utilization,
            IsActive = area.IsActive,
            CreationTime = area.CreationTime,
            CreatorId = area.CreatorId
        };
    }
}
