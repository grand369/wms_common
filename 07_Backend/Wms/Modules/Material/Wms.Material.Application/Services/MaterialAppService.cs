using Volo.Abp.Application.Services;
using Wms.Material.Application.Contracts.Dtos;
using Wms.Material.Application.Contracts.Permissions;
using Wms.Material.Application.Contracts.Services;
using MaterialAgg = Wms.Material.Domain.Aggregates.Material;
using Wms.Material.Domain.Aggregates;
using Wms.Material.Domain.Enums;
using Wms.Material.Domain.Repositories;
using Wms.Material.Domain.ValueObjects;

namespace Wms.Material.Application.Services;

/// <summary>
/// Material App Service — implements IMaterialAppService.
/// Maps value objects to/from flattened DTO properties.
/// (Phase 6 API Design)
/// </summary>
public class MaterialAppService : ApplicationService, IMaterialAppService
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMaterialClassificationRepository _classificationRepository;

    public MaterialAppService(
        IMaterialRepository materialRepository,
        IMaterialClassificationRepository classificationRepository)
    {
        _materialRepository = materialRepository;
        _classificationRepository = classificationRepository;
    }

    public async Task<MaterialOutputDto> GetAsync(Guid id)
    {
        var material = await _materialRepository.GetAsync(id);
        return MapToOutputDto(material);
    }

    public async Task<MaterialOutputDto> GetByCodeAsync(string materialCode)
    {
        var material = await _materialRepository.FindByCodeAsync(materialCode);
        if (material == null)
            throw new Volo.Abp.BusinessException("WMS:Material:NotFound").WithData("Code", materialCode);
        return MapToOutputDto(material);
    }

    public async Task<PagedResultDto<MaterialOutputDto>> GetListAsync(MaterialQueryDto query)
    {
        var queryable = await _materialRepository.GetQueryableAsync();
        var classificationQueryable = await _classificationRepository.GetQueryableAsync();

        var joinedQuery = from m in queryable
                          join c in classificationQueryable on m.ClassificationId equals c.Id into classificationGroup
                          from c in classificationGroup.DefaultIfEmpty()
                          select new { Material = m, Classification = c };

        joinedQuery = joinedQuery
            .WhereIf(!string.IsNullOrWhiteSpace(query.MaterialCode), x => x.Material.MaterialCode.Contains(query.MaterialCode!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.MaterialName), x => x.Material.MaterialName.Contains(query.MaterialName!))
            .WhereIf(query.MaterialType.HasValue, x => x.Material.MaterialType == query.MaterialType.Value)
            .WhereIf(query.ClassificationId.HasValue, x => x.Material.ClassificationId == query.ClassificationId.Value)
            .WhereIf(query.IsActive.HasValue, x => x.Material.IsActive == query.IsActive.Value)
            .WhereIf(query.ErpSyncStatus.HasValue, x => x.Material.ErpSyncStatus == query.ErpSyncStatus.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(joinedQuery);
        var items = await AsyncExecuter.ToListAsync(
            joinedQuery.OrderByDescending(x => x.Material.CreationTime).PageBy(query.PageIndex, query.PageSize));

        return new PagedResultDto<MaterialOutputDto>(totalCount, items.Select(x => MapToOutputDto(x.Material, x.Classification)).ToList());
    }

    [Authorize(WmsMaterialPermissions.Materials.Create)]
    public async Task<MaterialOutputDto> CreateAsync(MaterialCreateDto input)
    {
        if (await _materialRepository.CodeExistsAsync(input.MaterialCode))
            throw new Volo.Abp.BusinessException("WMS:Material:DuplicateCode").WithData("Code", input.MaterialCode);

        var material = new MaterialAgg(
            GuidGenerator.Create(),
            input.MaterialCode,
            input.MaterialName,
            input.MaterialType,
            input.PrimaryUnitId,
            input.PrimaryUnitName,
            new StorageAttribute(input.StorageConditionType, input.MaxStackingLayers, input.PackageSpec, input.WeightPerUnit),
            new QualityAttribute(input.BatchManagementEnabled, input.SerialManagementEnabled, input.ExpiryManagementEnabled, input.ShelfLifeDays, input.QualityInspectionMode),
            new InventoryAttribute(input.SafetyStockQuantity, input.MinOrderQuantity, input.ABCClassification, input.AllowNegativeInventory),
            new IssueStrategy(input.IssueStrategyType, input.StrategyScope),
            input.ErpSyncStatus,
            input.IsActive);

        material.SetMaterialNameEn(input.MaterialNameEn);
        material.SetClassificationId(input.ClassificationId);
        material.SetSpecification(input.Specification);
        material.SetSecondaryUnit(input.SecondaryUnitId, input.ConversionRate);
        material.SetPurchaseUnit(input.PurchaseUnitCode, input.PurchaseUnitName);
        material.SetInventoryUnit(input.InventoryUnitCode, input.InventoryUnitName);
        material.SetSalesUnit(input.SalesUnitCode, input.SalesUnitName);

        if (input.DangerLevel > 0 || !string.IsNullOrWhiteSpace(input.MSDSNumber))
        {
            material.UpdateDangerAttribute(new DangerAttribute(input.DangerLevel, input.MSDSNumber, input.SpecialMark));
        }

        await _materialRepository.InsertAsync(material);
        return MapToOutputDto(material);
    }

    [Authorize(WmsMaterialPermissions.Materials.Update)]
    public async Task<MaterialOutputDto> UpdateAsync(Guid id, MaterialUpdateDto input)
    {
        var material = await _materialRepository.GetAsync(id);

        material.SetMaterialName(input.MaterialName);
        material.SetType(input.MaterialType);
        material.SetMaterialNameEn(input.MaterialNameEn);
        material.SetClassificationId(input.ClassificationId);
        material.SetSpecification(input.Specification);
        material.SetPrimaryUnitName(input.PrimaryUnitName);
        material.SetSecondaryUnit(input.SecondaryUnitId, input.ConversionRate);
        material.SetPurchaseUnit(input.PurchaseUnitCode, input.PurchaseUnitName);
        material.SetInventoryUnit(input.InventoryUnitCode, input.InventoryUnitName);
        material.SetSalesUnit(input.SalesUnitCode, input.SalesUnitName);
        material.UpdateStorageAttribute(new StorageAttribute(input.StorageConditionType, input.MaxStackingLayers, input.PackageSpec, input.WeightPerUnit));
        material.UpdateQualityAttribute(new QualityAttribute(input.BatchManagementEnabled, input.SerialManagementEnabled, input.ExpiryManagementEnabled, input.ShelfLifeDays, input.QualityInspectionMode));
        material.UpdateInventoryAttribute(new InventoryAttribute(input.SafetyStockQuantity, input.MinOrderQuantity, input.ABCClassification, input.AllowNegativeInventory));
        material.UpdateIssueStrategy(new IssueStrategy(input.IssueStrategyType, input.StrategyScope));

        if (input.DangerLevel > 0 || !string.IsNullOrWhiteSpace(input.MSDSNumber))
            material.UpdateDangerAttribute(new DangerAttribute(input.DangerLevel, input.MSDSNumber, input.SpecialMark));
        else
            material.UpdateDangerAttribute(null);

        if (input.IsActive && !material.IsActive) material.SetActive();
        else if (!input.IsActive && material.IsActive) material.Deactivate();

        await _materialRepository.UpdateAsync(material);
        return MapToOutputDto(material);
    }

    [Authorize(WmsMaterialPermissions.Materials.Delete)]
    public async Task DeleteAsync(Guid id) => await _materialRepository.DeleteAsync(id);

    [Authorize(WmsMaterialPermissions.Materials.Activate)]
    public async Task ActivateAsync(Guid id)
    {
        var material = await _materialRepository.GetAsync(id);
        material.SetActive();
        await _materialRepository.UpdateAsync(material);
    }

    [Authorize(WmsMaterialPermissions.Materials.Deactivate)]
    public async Task DeactivateAsync(Guid id)
    {
        var material = await _materialRepository.GetAsync(id);
        material.Deactivate();
        await _materialRepository.UpdateAsync(material);
    }

    public async Task<List<MaterialSubstituteRelationDto>> GetSubstitutesAsync(Guid materialId)
    {
        var material = await _materialRepository.GetAsync(materialId);
        return material.SubstituteRelations.Select(r => new MaterialSubstituteRelationDto
        {
            Id = r.Id,
            OriginalMaterialId = r.OriginalMaterialId,
            SubstituteMaterialId = r.SubstituteMaterialId,
            SubstituteMaterialCode = r.SubstituteMaterialCode,
            SubstitutePriority = r.SubstitutePriority,
            SubstituteRatio = r.SubstituteRatio
        }).ToList();
    }

    [Authorize(WmsMaterialPermissions.Substitutes.Create)]
    public async Task<MaterialSubstituteRelationDto> AddSubstituteAsync(Guid materialId, Guid substituteMaterialId, string substituteMaterialCode, int priority = 1, decimal ratio = 1.0m)
    {
        var material = await _materialRepository.GetAsync(materialId);
        material.AddSubstituteRelation(substituteMaterialId, substituteMaterialCode, priority, ratio);
        await _materialRepository.UpdateAsync(material);
        var relation = material.SubstituteRelations.First(r => r.SubstituteMaterialId == substituteMaterialId);
        return new MaterialSubstituteRelationDto
        {
            Id = relation.Id,
            OriginalMaterialId = relation.OriginalMaterialId,
            SubstituteMaterialId = relation.SubstituteMaterialId,
            SubstituteMaterialCode = relation.SubstituteMaterialCode,
            SubstitutePriority = relation.SubstitutePriority,
            SubstituteRatio = relation.SubstituteRatio
        };
    }

    [Authorize(WmsMaterialPermissions.Substitutes.Delete)]
    public async Task RemoveSubstituteAsync(Guid materialId, Guid substituteRelationId)
    {
        var material = await _materialRepository.GetAsync(materialId);
        material.RemoveSubstituteRelation(substituteRelationId);
        await _materialRepository.UpdateAsync(material);
    }

    private MaterialOutputDto MapToOutputDto(MaterialAgg material)
    {
        return MapToOutputDto(material, null);
    }

    private MaterialOutputDto MapToOutputDto(MaterialAgg material, MaterialClassification? classification)
    {
        var typeEnum = Domain.Enums.MaterialType.FromValue(material.MaterialType);
        var storageEnum = Domain.Enums.StorageConditionType.FromValue(material.StorageAttribute.StorageConditionType);
        var inspectionEnum = Domain.Enums.QualityInspectionMode.FromValue(material.QualityAttribute.QualityInspectionMode);
        var abcEnum = Domain.Enums.ABCClassificationType.FromValue(material.InventoryAttribute.ABCClassification);
        var issueEnum = Domain.Enums.IssueStrategyType.FromValue(material.IssueStrategy.IssueStrategyType);
        var scopeEnum = Domain.Enums.StrategyScope.FromValue(material.IssueStrategy.StrategyScope);
        var erpEnum = Domain.Enums.ErpSyncStatus.FromValue(material.ErpSyncStatus);

        string? dangerLevelDesc = null;
        if (material.DangerAttribute != null)
            dangerLevelDesc = Domain.Enums.DangerLevelType.FromValue(material.DangerAttribute.DangerLevel).Description;

        return new MaterialOutputDto
        {
            Id = material.Id,
            MaterialCode = material.MaterialCode,
            MaterialName = material.MaterialName,
            MaterialNameEn = material.MaterialNameEn,
            ClassificationId = material.ClassificationId,
            ClassificationName = classification?.ClassificationName,
            Specification = material.Specification,
            PrimaryUnitId = material.PrimaryUnitId,
            PrimaryUnitName = material.PrimaryUnitName,
            SecondaryUnitId = material.SecondaryUnitId,
            ConversionRate = material.ConversionRate,
            PurchaseUnitCode = material.PurchaseUnitCode,
            PurchaseUnitName = material.PurchaseUnitName,
            InventoryUnitCode = material.InventoryUnitCode,
            InventoryUnitName = material.InventoryUnitName,
            SalesUnitCode = material.SalesUnitCode,
            SalesUnitName = material.SalesUnitName,
            MaterialType = material.MaterialType,
            MaterialTypeDescription = typeEnum.Description,
            StorageConditionType = material.StorageAttribute.StorageConditionType,
            StorageConditionTypeDescription = storageEnum.Description,
            MaxStackingLayers = material.StorageAttribute.MaxStackingLayers,
            PackageSpec = material.StorageAttribute.PackageSpec,
            WeightPerUnit = material.StorageAttribute.WeightPerUnit,
            BatchManagementEnabled = material.QualityAttribute.BatchManagementEnabled,
            SerialManagementEnabled = material.QualityAttribute.SerialManagementEnabled,
            ExpiryManagementEnabled = material.QualityAttribute.ExpiryManagementEnabled,
            ShelfLifeDays = material.QualityAttribute.ShelfLifeDays,
            QualityInspectionMode = material.QualityAttribute.QualityInspectionMode,
            QualityInspectionModeDescription = inspectionEnum.Description,
            SafetyStockQuantity = material.InventoryAttribute.SafetyStockQuantity,
            MinOrderQuantity = material.InventoryAttribute.MinOrderQuantity,
            ABCClassification = material.InventoryAttribute.ABCClassification,
            ABCClassificationDescription = abcEnum.Description,
            AllowNegativeInventory = material.InventoryAttribute.AllowNegativeInventory,
            IssueStrategyType = material.IssueStrategy.IssueStrategyType,
            IssueStrategyTypeDescription = issueEnum.Description,
            StrategyScope = material.IssueStrategy.StrategyScope,
            StrategyScopeDescription = scopeEnum.Description,
            DangerLevel = material.DangerAttribute?.DangerLevel ?? 0,
            DangerLevelDescription = dangerLevelDesc ?? string.Empty,
            MSDSNumber = material.DangerAttribute?.MSDSNumber ?? string.Empty,
            SpecialMark = material.DangerAttribute?.SpecialMark ?? string.Empty,
            IsActive = material.IsActive,
            ErpSyncStatus = material.ErpSyncStatus,
            ErpSyncStatusDescription = erpEnum.Description,
            SubstituteRelations = material.SubstituteRelations.Select(r => new MaterialSubstituteRelationDto
            {
                Id = r.Id, OriginalMaterialId = r.OriginalMaterialId, SubstituteMaterialId = r.SubstituteMaterialId,
                SubstituteMaterialCode = r.SubstituteMaterialCode, SubstitutePriority = r.SubstitutePriority, SubstituteRatio = r.SubstituteRatio
            }).ToList(),
            CreationTime = material.CreationTime,
            CreatorId = material.CreatorId
        };
    }
}
