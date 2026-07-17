using Volo.Abp.Application.Services;
using Wms.Material.Application.Contracts.Dtos;
using Wms.Material.Application.Contracts.Permissions;
using Wms.Material.Application.Contracts.Services;
using Wms.Material.Domain.Entities;
using Wms.Material.Domain.Enums;
using Wms.Material.Domain.Repositories;

namespace Wms.Material.Application.Services;

/// <summary>
/// Unit of Measure App Service — implements IUnitOfMeasureAppService.
/// </summary>
public class UnitOfMeasureAppService : ApplicationService, IUnitOfMeasureAppService
{
    private readonly IUnitOfMeasureRepository _unitRepository;

    public UnitOfMeasureAppService(IUnitOfMeasureRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    public async Task<UnitOfMeasureOutputDto> GetAsync(Guid id)
    {
        var unit = await _unitRepository.GetAsync(id);
        return MapToOutputDto(unit);
    }

    public async Task<UnitOfMeasureOutputDto> GetByCodeAsync(string unitCode)
    {
        var unit = await _unitRepository.FindByCodeAsync(unitCode);
        if (unit == null)
            throw new Volo.Abp.BusinessException("WMS:Material:UnitNotFound").WithData("Code", unitCode);
        return MapToOutputDto(unit);
    }

    public async Task<PagedResultDto<UnitOfMeasureOutputDto>> GetListAsync(UnitOfMeasureQueryDto query)
    {
        var queryable = await _unitRepository.GetQueryableAsync();
        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(query.UnitCode), u => u.UnitCode.Contains(query.UnitCode!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.UnitName), u => u.UnitName.Contains(query.UnitName!))
            .WhereIf(query.UnitType.HasValue, u => u.UnitType == query.UnitType.Value)
            .WhereIf(query.IsActive.HasValue, u => u.IsActive == query.IsActive.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(queryable.OrderBy(u => u.UnitCode).PageBy(query.PageIndex, query.PageSize));
        return new PagedResultDto<UnitOfMeasureOutputDto>(totalCount, items.Select(MapToOutputDto).ToList());
    }

    public async Task<List<UnitOfMeasureOutputDto>> GetActiveListAsync()
    {
        var units = await _unitRepository.GetActiveListAsync();
        return units.Select(MapToOutputDto).ToList();
    }

    [Authorize(WmsMaterialPermissions.Units.Create)]
    public async Task<UnitOfMeasureOutputDto> CreateAsync(UnitOfMeasureCreateDto input)
    {
        if (await _unitRepository.CodeExistsAsync(input.UnitCode))
            throw new Volo.Abp.BusinessException("WMS:Material:DuplicateUnitCode").WithData("Code", input.UnitCode);

        var unit = new UnitOfMeasure(GuidGenerator.Create(), input.UnitCode, input.UnitName, input.UnitSymbol, input.UnitType, input.IsActive);
        await _unitRepository.InsertAsync(unit);
        return MapToOutputDto(unit);
    }

    [Authorize(WmsMaterialPermissions.Units.Update)]
    public async Task<UnitOfMeasureOutputDto> UpdateAsync(Guid id, UnitOfMeasureUpdateDto input)
    {
        var unit = await _unitRepository.GetAsync(id);
        unit.SetUnitName(input.UnitName);
        unit.SetUnitSymbol(input.UnitSymbol);
        unit.SetUnitType(input.UnitType);
        if (input.IsActive && !unit.IsActive) unit.SetActive();
        else if (!input.IsActive && unit.IsActive) unit.Deactivate();
        await _unitRepository.UpdateAsync(unit);
        return MapToOutputDto(unit);
    }

    [Authorize(WmsMaterialPermissions.Units.Delete)]
    public async Task DeleteAsync(Guid id) => await _unitRepository.DeleteAsync(id);

    private UnitOfMeasureOutputDto MapToOutputDto(UnitOfMeasure unit)
    {
        var typeEnum = Domain.Enums.UnitType.FromValue(unit.UnitType);
        return new UnitOfMeasureOutputDto
        {
            Id = unit.Id, UnitCode = unit.UnitCode, UnitName = unit.UnitName,
            UnitSymbol = unit.UnitSymbol, UnitType = unit.UnitType,
            UnitTypeDescription = typeEnum.Description, IsActive = unit.IsActive,
            CreationTime = unit.CreationTime
        };
    }
}
