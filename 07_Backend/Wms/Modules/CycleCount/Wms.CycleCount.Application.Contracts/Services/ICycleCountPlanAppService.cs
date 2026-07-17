using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Wms.CycleCount.Application.Contracts.Dtos;

namespace Wms.CycleCount.Application.Contracts.Services;

/// <summary>ICycleCountPlanAppService — 9 API methods (3 CRUD + 6 business)</summary>
public interface ICycleCountPlanAppService : IApplicationService
{
    Task<PagedResultDto<CycleCountPlanOutputDto>> GetListAsync(CycleCountPlanQueryDto query);
    Task<CycleCountPlanOutputDto> GetAsync(Guid id);
    Task<CycleCountPlanOutputDto> CreateAsync(CycleCountPlanCreateDto input);
    Task<CycleCountPlanOutputDto> StartCountingAsync(Guid id);
    Task<CycleCountPlanOutputDto> SubmitCountAsync(Guid id, List<SubmitCountCommandDto> items);
    Task<CycleCountPlanOutputDto> RecountAsync(Guid id, Guid itemId);
    Task<CycleCountPlanOutputDto> ConfirmDifferenceAsync(Guid id);
    Task<CycleCountPlanOutputDto> GenerateAdjustmentAsync(Guid id);
    Task<CycleCountPlanOutputDto> CompleteAsync(Guid id);
}
