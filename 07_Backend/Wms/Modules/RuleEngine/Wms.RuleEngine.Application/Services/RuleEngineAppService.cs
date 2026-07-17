using AutoMapper;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Wms.RuleEngine.Application.Contracts.Dtos;
using Wms.RuleEngine.Application.Contracts.Permissions;
using Wms.RuleEngine.Application.Contracts.Services;
using Wms.RuleEngine.Domain.Aggregates;
using Wms.RuleEngine.Domain.Enums;
using Wms.RuleEngine.Domain.Services;

namespace Wms.RuleEngine.Application.Services;

/// <summary>
/// RuleEngineAppService — application service for business rule and industry package operations.
/// Implements all 7 methods from IBusinessRuleAppService (API-RE-001~007).
/// </summary>
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IBusinessRuleAppService))]
public class RuleEngineAppService : ApplicationService, IBusinessRuleAppService
{
    private readonly IRepository<BusinessRule, Guid> _businessRuleRepository;
    private readonly IRepository<IndustryPackage, Guid> _industryPackageRepository;
    private readonly RuleEngineDomainService _ruleEngineDomainService;
    private readonly IMapper _mapper;

    public RuleEngineAppService(
        IRepository<BusinessRule, Guid> businessRuleRepository,
        IRepository<IndustryPackage, Guid> industryPackageRepository,
        RuleEngineDomainService ruleEngineDomainService,
        IMapper mapper)
    {
        _businessRuleRepository = businessRuleRepository;
        _industryPackageRepository = industryPackageRepository;
        _ruleEngineDomainService = ruleEngineDomainService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get paged list of business rules (API-RE-001).
    /// </summary>
    [Authorize(WmsRuleEnginePermissions.Read)]
    public async Task<PagedResultDto<BusinessRuleOutputDto>> GetListAsync(BusinessRuleQueryDto query)
    {
        var queryable = await _businessRuleRepository.GetQueryableAsync();

        if (query.RuleTypeValue.HasValue)
        {
            var ruleType = RuleType.FromValue(query.RuleTypeValue.Value);
            queryable = queryable.Where(r => r.RuleType == ruleType);
        }

        if (query.EffectiveStatusValue.HasValue)
        {
            var status = EffectiveStatus.FromValue(query.EffectiveStatusValue.Value);
            queryable = queryable.Where(r => r.EffectiveStatus == status);
        }

        if (!string.IsNullOrWhiteSpace(query.RuleName))
        {
            queryable = queryable.Where(r => r.RuleName.Contains(query.RuleName));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(r => r.CreationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<BusinessRuleOutputDto>(totalCount,
            _mapper.Map<List<BusinessRuleOutputDto>>(items));
    }

    /// <summary>
    /// Get a business rule by ID (API-RE-002).
    /// </summary>
    [Authorize(WmsRuleEnginePermissions.Read)]
    public async Task<BusinessRuleOutputDto> GetAsync(Guid id)
    {
        var rule = await _businessRuleRepository.GetAsync(id);
        return _mapper.Map<BusinessRuleOutputDto>(rule);
    }

    /// <summary>
    /// Create a business rule (API-RE-003).
    /// </summary>
    [Authorize(WmsRuleEnginePermissions.Create)]
    public async Task<BusinessRuleOutputDto> CreateAsync(BusinessRuleCreateDto dto)
    {
        var ruleType = RuleType.FromValue(dto.RuleTypeValue);

        var rule = new BusinessRule(
            GuidGenerator.Create(),
            dto.RuleName,
            ruleType,
            dto.RuleCondition,
            dto.RuleAction,
            dto.Description,
            dto.EffectiveFrom,
            dto.EffectiveTo
        );

        await _businessRuleRepository.InsertAsync(rule);
        return _mapper.Map<BusinessRuleOutputDto>(rule);
    }

    /// <summary>
    /// Update a business rule (API-RE-004).
    /// </summary>
    [Authorize(WmsRuleEnginePermissions.Update)]
    public async Task<BusinessRuleOutputDto> UpdateAsync(Guid id, BusinessRuleUpdateDto dto)
    {
        var rule = await _businessRuleRepository.GetAsync(id);

        rule.UpdateMetadata(dto.RuleName, dto.Description);
        rule.UpdateCondition(dto.RuleCondition);
        rule.UpdateAction(dto.RuleAction);
        rule.IncrementVersion();

        var effectiveStatus = EffectiveStatus.FromValue(dto.EffectiveStatusValue);
        if (effectiveStatus == EffectiveStatus.Active)
        {
            rule.Activate();
        }
        else if (effectiveStatus == EffectiveStatus.Inactive)
        {
            rule.Deactivate();
        }

        rule.SetEffectivePeriod(dto.EffectiveFrom, dto.EffectiveTo);

        await _businessRuleRepository.UpdateAsync(rule);
        return _mapper.Map<BusinessRuleOutputDto>(rule);
    }

    /// <summary>
    /// Evaluate a business rule with context data (API-RE-005).
    /// </summary>
    [Authorize(WmsRuleEnginePermissions.Execute)]
    public async Task<RuleEvaluateResultDto> EvaluateAsync(Guid id, RuleEvaluateDto dto)
    {
        var result = await _ruleEngineDomainService.EvaluateRuleAsync(dto.RuleName, dto.ContextData);

        return new RuleEvaluateResultDto
        {
            RuleName = dto.RuleName,
            Result = result,
            EvaluatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Get paged list of industry packages (API-RE-006).
    /// </summary>
    [Authorize(WmsRuleEnginePermissions.Read)]
    public async Task<PagedResultDto<IndustryPackageOutputDto>> GetPackageListAsync(IndustryPackageQueryDto query)
    {
        var queryable = await _industryPackageRepository.GetQueryableAsync();

        if (query.IndustryTypeValue.HasValue)
        {
            var industryType = IndustryType.FromValue(query.IndustryTypeValue.Value);
            queryable = queryable.Where(p => p.IndustryType == industryType);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(p => p.CreationTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<IndustryPackageOutputDto>(totalCount,
            _mapper.Map<List<IndustryPackageOutputDto>>(items));
    }

    /// <summary>
    /// Import an industry package — parse rules and insert into system (API-RE-007).
    /// </summary>
    [Authorize(WmsRuleEnginePermissions.Import)]
    public async Task<List<BusinessRuleOutputDto>> ImportPackageAsync(Guid packageId)
    {
        var importedRuleNames = await _ruleEngineDomainService.ImportIndustryPackageAsync(packageId);

        // Reload imported rules to return full output
        var results = new List<BusinessRuleOutputDto>();
        foreach (var ruleName in importedRuleNames)
        {
            // Query by name — rules were just created
            var queryable = await _businessRuleRepository.GetQueryableAsync();
            var rule = await AsyncExecuter.FirstOrDefaultAsync(
                queryable.Where(r => r.RuleName == ruleName));
            if (rule != null)
            {
                results.Add(_mapper.Map<BusinessRuleOutputDto>(rule));
            }
        }

        return results;
    }
}
