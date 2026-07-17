using Volo.Abp.Application.Services;
using Wms.Material.Application.Contracts.Dtos;
using Wms.Material.Application.Contracts.Services;
using Wms.Material.Domain.Aggregates;
using Wms.Material.Domain.Repositories;

namespace Wms.Material.Application.Services;

public class IssueStrategyAppService : ApplicationService, IIssueStrategyAppService
{
    private readonly IMaterialIssueStrategyRepository _strategyRepository;

    public IssueStrategyAppService(IMaterialIssueStrategyRepository strategyRepository)
    {
        _strategyRepository = strategyRepository;
    }

    public async Task<MaterialIssueStrategyOutputDto> GetAsync(Guid id)
    {
        var strategy = await _strategyRepository.GetAsync(id);
        return MapToOutputDto(strategy);
    }

    public async Task<PagedResultDto<MaterialIssueStrategyOutputDto>> GetListAsync(MaterialIssueStrategyQueryDto query)
    {
        var queryable = await _strategyRepository.GetQueryableAsync();
        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(query.Filter), 
                s => s.Code.Contains(query.Filter!) || s.Name.Contains(query.Filter!));

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(queryable
            .OrderBy(s => s.Code)
            .PageBy(query.SkipCount, query.MaxResultCount));

        return new PagedResultDto<MaterialIssueStrategyOutputDto>(totalCount, items.Select(MapToOutputDto).ToList());
    }

    public async Task<MaterialIssueStrategyOutputDto> CreateAsync(MaterialIssueStrategyCreateDto input)
    {
        if (await _strategyRepository.CodeExistsAsync(input.Code))
            throw new Volo.Abp.BusinessException("WMS:Material:DuplicateStrategyCode").WithData("Code", input.Code);

        var strategy = new MaterialIssueStrategy(
            GuidGenerator.Create(),
            input.Code,
            input.Name,
            input.Strategy,
            input.Description);

        await _strategyRepository.InsertAsync(strategy);
        return MapToOutputDto(strategy);
    }

    public async Task<MaterialIssueStrategyOutputDto> UpdateAsync(Guid id, MaterialIssueStrategyUpdateDto input)
    {
        var strategy = await _strategyRepository.GetAsync(id);
        strategy.Update(input.Name, input.Strategy, input.Description);
        await _strategyRepository.UpdateAsync(strategy);
        return MapToOutputDto(strategy);
    }

    public async Task DeleteAsync(Guid id) => await _strategyRepository.DeleteAsync(id);

    private MaterialIssueStrategyOutputDto MapToOutputDto(MaterialIssueStrategy strategy)
    {
        return new MaterialIssueStrategyOutputDto
        {
            Id = strategy.Id,
            Code = strategy.Code,
            Name = strategy.Name,
            Strategy = strategy.Strategy,
            Description = strategy.Description
        };
    }
}