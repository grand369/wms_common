using Volo.Abp.Application.Services;
using Wms.Warehouse.Application.Contracts.Dtos;
using Wms.Warehouse.Application.Contracts.Permissions;
using Wms.Warehouse.Application.Contracts.Services;
using WarehouseAgg = Wms.Warehouse.Domain.Aggregates.Warehouse;
using Wms.Warehouse.Domain.Enums;
using Wms.Warehouse.Domain.Repositories;
using Wms.Warehouse.Domain.Services;

namespace Wms.Warehouse.Application.Services;

/// <summary>
/// Warehouse App Service — implements IWarehouseAppService using IWarehouseRepository and WarehouseManager.
/// (Phase 6 API Design)
/// </summary>
public class WarehouseAppService : ApplicationService, IWarehouseAppService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly WarehouseManager _warehouseManager;

    public WarehouseAppService(
        IWarehouseRepository warehouseRepository,
        WarehouseManager warehouseManager)
    {
        _warehouseRepository = warehouseRepository;
        _warehouseManager = warehouseManager;
    }

    public async Task<WarehouseOutputDto> GetAsync(Guid id)
    {
        var warehouse = await _warehouseRepository.GetAsync(id);
        return MapToOutputDto(warehouse);
    }

    public async Task<WarehouseOutputDto> GetByCodeAsync(string warehouseCode)
    {
        var warehouse = await _warehouseRepository.FindByCodeAsync(warehouseCode);
        if (warehouse == null)
            throw new Volo.Abp.BusinessException("WMS:Warehouse:NotFound").WithData("Code", warehouseCode);
        return MapToOutputDto(warehouse);
    }

    public async Task<PagedResultDto<WarehouseOutputDto>> GetListAsync(WarehouseQueryDto query)
    {
        var queryable = await _warehouseRepository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(query.WarehouseCode),
                w => w.WarehouseCode.Contains(query.WarehouseCode!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.WarehouseName),
                w => w.WarehouseName.Contains(query.WarehouseName!))
            .WhereIf(query.WarehouseType.HasValue,
                w => w.WarehouseType == query.WarehouseType.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(query.OrganizationUnitId),
                w => w.OrganizationUnitId == query.OrganizationUnitId)
            .WhereIf(!string.IsNullOrWhiteSpace(query.PlantId),
                w => w.PlantId == query.PlantId)
            .WhereIf(query.IsActive.HasValue,
                w => w.IsActive == query.IsActive.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(w => w.CreationTime)
                     .PageBy(query.PageIndex, query.PageSize));

        return new PagedResultDto<WarehouseOutputDto>(
            totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    public async Task<List<WarehouseOutputDto>> GetAllListAsync()
    {
        var warehouses = await _warehouseRepository.GetActiveListAsync();
        return warehouses.Select(MapToOutputDto).ToList();
    }

    [Authorize(WmsWarehousePermissions.Warehouses.Create)]
    public async Task<WarehouseOutputDto> CreateAsync(WarehouseCreateDto input)
    {
        await _warehouseManager.ValidateWarehouseCodeUniqueAsync(input.WarehouseCode);

        var warehouse = new WarehouseAgg(
            GuidGenerator.Create(),
            input.WarehouseCode,
            input.WarehouseName,
            input.WarehouseType,
            input.OrganizationUnitId,
            input.OrganizationUnitName,
            input.PlantId,
            input.PlantName,
            input.StorageConditionType,
            input.LocationLevelCount,
            input.IsActive);

        warehouse.SetResponsibleUser(input.ResponsibleUserId, input.ResponsibleUserName);
        warehouse.SetAddress(input.Address);
        warehouse.SetRemark(input.Remark);

        var t = await _warehouseRepository.InsertAsync(warehouse);
        return MapToOutputDto(warehouse);
    }

    [Authorize(WmsWarehousePermissions.Warehouses.Update)]
    public async Task<WarehouseOutputDto> UpdateAsync(Guid id, WarehouseUpdateDto input)
    {
        var warehouse = await _warehouseRepository.GetAsync(id);

        warehouse.SetWarehouseName(input.WarehouseName);
        warehouse.SetType(input.WarehouseType);
        warehouse.SetOrganizationUnitName(input.OrganizationUnitName);
        warehouse.SetPlantName(input.PlantName);
        warehouse.SetResponsibleUser(input.ResponsibleUserId, input.ResponsibleUserName);
        warehouse.SetAddress(input.Address);
        warehouse.SetStorageConditionType(input.StorageConditionType);
        warehouse.SetLocationLevelCount(input.LocationLevelCount);
        warehouse.SetRemark(input.Remark);

        if (input.IsActive && !warehouse.IsActive)
            warehouse.SetActive();
        else if (!input.IsActive && warehouse.IsActive)
            warehouse.Deactivate();

        await _warehouseRepository.UpdateAsync(warehouse);
        return MapToOutputDto(warehouse);
    }

    [Authorize(WmsWarehousePermissions.Warehouses.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _warehouseRepository.DeleteAsync(id);
    }

    [Authorize(WmsWarehousePermissions.Warehouses.Activate)]
    public async Task ActivateAsync(Guid id)
    {
        var warehouse = await _warehouseRepository.GetAsync(id);
        warehouse.SetActive();
        await _warehouseRepository.UpdateAsync(warehouse);
    }

    [Authorize(WmsWarehousePermissions.Warehouses.Deactivate)]
    public async Task DeactivateAsync(Guid id)
    {
        var warehouse = await _warehouseRepository.GetAsync(id);
        warehouse.Deactivate();
        await _warehouseRepository.UpdateAsync(warehouse);
    }

    private WarehouseOutputDto MapToOutputDto(WarehouseAgg warehouse)
    {
        var typeEnum = WarehouseType.FromValue(warehouse.WarehouseType);
        var conditionEnum = StorageConditionType.FromValue(warehouse.StorageConditionType);

        return new WarehouseOutputDto
        {
            Id = warehouse.Id,
            WarehouseCode = warehouse.WarehouseCode,
            WarehouseName = warehouse.WarehouseName,
            WarehouseType = warehouse.WarehouseType,
            WarehouseTypeDescription = typeEnum.Description,
            OrganizationUnitId = warehouse.OrganizationUnitId,
            OrganizationUnitName = warehouse.OrganizationUnitName,
            PlantId = warehouse.PlantId,
            PlantName = warehouse.PlantName,
            ResponsibleUserId = warehouse.ResponsibleUserId,
            ResponsibleUserName = warehouse.ResponsibleUserName,
            Address = warehouse.Address,
            StorageConditionType = warehouse.StorageConditionType,
            StorageConditionTypeDescription = conditionEnum.Description,
            LocationLevelCount = warehouse.LocationLevelCount,
            IsActive = warehouse.IsActive,
            Remark = warehouse.Remark,
            CreationTime = warehouse.CreationTime,
            CreatorId = warehouse.CreatorId
        };
    }
}
