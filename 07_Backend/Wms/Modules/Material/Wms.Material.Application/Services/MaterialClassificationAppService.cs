using Volo.Abp.Application.Services;
using Wms.Material.Application.Contracts.Dtos;
using Wms.Material.Application.Contracts.Permissions;
using Wms.Material.Application.Contracts.Services;
using Wms.Material.Domain.Aggregates;
using Wms.Material.Domain.Enums;
using Wms.Material.Domain.Repositories;

namespace Wms.Material.Application.Services;

/// <summary>
/// Material Classification App Service — implements IMaterialClassificationAppService.
/// </summary>
public class MaterialClassificationAppService : ApplicationService, IMaterialClassificationAppService
{
    private readonly IMaterialClassificationRepository _classificationRepository;

    public MaterialClassificationAppService(IMaterialClassificationRepository classificationRepository)
    {
        _classificationRepository = classificationRepository;
    }

    public async Task<MaterialClassificationOutputDto> GetAsync(Guid id)
    {
        var classification = await _classificationRepository.GetAsync(id);
        return MapToOutputDto(classification);
    }

    public async Task<MaterialClassificationOutputDto> GetByCodeAsync(string classificationCode)
    {
        var classification = await _classificationRepository.FindByCodeAsync(classificationCode);
        if (classification == null)
            throw new Volo.Abp.BusinessException("WMS:Material:ClassificationNotFound").WithData("Code", classificationCode);
        return MapToOutputDto(classification);
    }

    public async Task<PagedResultDto<MaterialClassificationOutputDto>> GetListAsync(MaterialClassificationQueryDto query)
    {
        var queryable = await _classificationRepository.GetQueryableAsync();
        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(query.ClassificationCode), c => c.ClassificationCode.Contains(query.ClassificationCode!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.ClassificationName), c => c.ClassificationName.Contains(query.ClassificationName!))
            .WhereIf(query.ParentClassificationId.HasValue, c => c.ParentClassificationId == query.ParentClassificationId.Value)
            .WhereIf(query.ClassificationLevel.HasValue, c => c.ClassificationLevel == query.ClassificationLevel.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(queryable.OrderBy(c => c.ClassificationLevel).ThenBy(c => c.ClassificationCode).PageBy(query.SkipCount, query.MaxResultCount));
        return new PagedResultDto<MaterialClassificationOutputDto>(totalCount, items.Select(MapToOutputDto).ToList());
    }

    public async Task<List<MaterialClassificationOutputDto>> GetTreeAsync()
    {
        var allClassifications = await _classificationRepository.GetTreeAsync();
        var rootClassifications = allClassifications.Where(c => c.ParentClassificationId == null).ToList();
        return rootClassifications.Select(BuildTree).ToList();
    }

    [Authorize(WmsMaterialPermissions.Classifications.Create)]
    public async Task<MaterialClassificationOutputDto> CreateAsync(MaterialClassificationCreateDto input)
    {
        if (await _classificationRepository.CodeExistsAsync(input.ClassificationCode))
            throw new Volo.Abp.BusinessException("WMS:Material:DuplicateClassificationCode").WithData("Code", input.ClassificationCode);

        var classification = new MaterialClassification(
            GuidGenerator.Create(),
            input.ClassificationCode,
            input.ClassificationName,
            input.ParentClassificationId,
            input.ClassificationLevel,
            input.AttributeTemplateId);

        await _classificationRepository.InsertAsync(classification);
        return MapToOutputDto(classification);
    }

    [Authorize(WmsMaterialPermissions.Classifications.Update)]
    public async Task<MaterialClassificationOutputDto> UpdateAsync(Guid id, MaterialClassificationUpdateDto input)
    {
        var classification = await _classificationRepository.GetAsync(id);
        classification.SetClassificationName(input.ClassificationName);
        classification.UpdateParent(input.ParentClassificationId, input.ClassificationLevel);
        classification.SetAttributeTemplateId(input.AttributeTemplateId);
        await _classificationRepository.UpdateAsync(classification);
        return MapToOutputDto(classification);
    }

    [Authorize(WmsMaterialPermissions.Classifications.Delete)]
    public async Task DeleteAsync(Guid id) => await _classificationRepository.DeleteAsync(id);

    private MaterialClassificationOutputDto MapToOutputDto(MaterialClassification classification)
    {
        return new MaterialClassificationOutputDto
        {
            Id = classification.Id,
            ClassificationCode = classification.ClassificationCode,
            ClassificationName = classification.ClassificationName,
            ParentClassificationId = classification.ParentClassificationId,
            ClassificationLevel = classification.ClassificationLevel,
            AttributeTemplateId = classification.AttributeTemplateId,
            Children = new List<MaterialClassificationOutputDto>(),
            CreationTime = classification.CreationTime
        };
    }

    private MaterialClassificationOutputDto BuildTree(MaterialClassification classification)
    {
        var dto = MapToOutputDto(classification);
        var children = _classificationRepository.GetListByParentIdAsync(classification.Id).GetAwaiter().GetResult();
        dto.Children = children.Select(BuildTree).ToList();
        return dto;
    }
}
