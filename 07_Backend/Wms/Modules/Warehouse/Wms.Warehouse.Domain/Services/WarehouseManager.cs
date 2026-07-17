using Volo.Abp.Domain.Services;
using Wms.Warehouse.Domain.Aggregates;
using Wms.Warehouse.Domain.Repositories;

namespace Wms.Warehouse.Domain.Services;

/// <summary>
/// Warehouse Manager Domain Service — provides cross-aggregate coordination and validation.
/// Validates uniqueness of codes and location putaway compatibility.
/// (Phase 3 DDD Design, Section 8)
/// </summary>
public class WarehouseManager : DomainService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWarehouseAreaRepository _areaRepository;
    private readonly ILocationRepository _locationRepository;

    public WarehouseManager(
        IWarehouseRepository warehouseRepository,
        IWarehouseAreaRepository areaRepository,
        ILocationRepository locationRepository)
    {
        _warehouseRepository = warehouseRepository;
        _areaRepository = areaRepository;
        _locationRepository = locationRepository;
    }

    /// <summary>
    /// Validates that a warehouse code is unique before creation or update.
    /// Throws BusinessException if the code already exists.
    /// </summary>
    public async Task ValidateWarehouseCodeUniqueAsync(string warehouseCode, Guid? excludeId = null)
    {
        var existing = await _warehouseRepository.FindByCodeAsync(warehouseCode);
        if (existing != null && existing.Id != excludeId)
        {
            throw new Volo.Abp.BusinessException("WMS:Warehouse:DuplicateCode")
                .WithData("Code", warehouseCode);
        }
    }

    /// <summary>
    /// Validates that a location code is unique before creation or update.
    /// Throws BusinessException if the code already exists.
    /// </summary>
    public async Task ValidateLocationCodeUniqueAsync(string locationCode, Guid? excludeId = null)
    {
        var existing = await _locationRepository.FindByCodeAsync(locationCode);
        if (existing != null && existing.Id != excludeId)
        {
            throw new Volo.Abp.BusinessException("WMS:Warehouse:DuplicateLocationCode")
                .WithData("Code", locationCode);
        }
    }

    /// <summary>
    /// Validates that an area code is unique within a warehouse before creation or update.
    /// Throws BusinessException if the code already exists in that warehouse.
    /// </summary>
    public async Task ValidateAreaCodeUniqueAsync(string areaCode, string warehouseId, Guid? excludeId = null)
    {
        var existing = await _areaRepository.FindByCodeAndWarehouseIdAsync(areaCode, warehouseId);
        if (existing != null && existing.Id != excludeId)
        {
            throw new Volo.Abp.BusinessException("WMS:Warehouse:DuplicateAreaCode")
                .WithData("Code", areaCode)
                .WithData("WarehouseId", warehouseId);
        }
    }

    /// <summary>
    /// Validates putaway compatibility between a location and a material's storage condition.
    /// Uses the Location aggregate's ValidatePutawayCompatibility method.
    /// </summary>
    public async Task<bool> ValidateLocationPutawayCompatibilityAsync(Guid locationId, int materialStorageCondition)
    {
        var location = await _locationRepository.FindAsync(locationId);
        if (location == null)
        {
            throw new Volo.Abp.BusinessException("WMS:Warehouse:LocationNotFound")
                .WithData("LocationId", locationId);
        }
        return location.ValidatePutawayCompatibility(materialStorageCondition);
    }
}
