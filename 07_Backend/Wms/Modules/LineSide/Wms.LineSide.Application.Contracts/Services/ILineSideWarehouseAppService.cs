using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Wms.LineSide.Application.Contracts.Dtos;

namespace Wms.LineSide.Application.Contracts.Services;

/// <summary>ILineSideWarehouseAppService — 7 API methods (4 CRUD + 3 business)</summary>
public interface ILineSideWarehouseAppService : IApplicationService
{
    Task<PagedResultDto<LineSideWarehouseOutputDto>> GetListAsync(LineSideWarehouseQueryDto query);
    Task<LineSideWarehouseOutputDto> GetAsync(Guid id);
    Task<LineSideWarehouseOutputDto> CreateAsync(LineSideWarehouseCreateDto input);
    Task<LineSideWarehouseOutputDto> UpdateAsync(Guid id, LineSideWarehouseCreateDto input);
    Task<List<LineSideKanbanItemOutputDto>> GetKanbanItemsAsync(Guid id);
    Task<LineSideWarehouseOutputDto> TriggerReplenishmentAsync(Guid id, TriggerReplenishmentCommandDto input);
    Task<LineSideWarehouseOutputDto> BackflushConsumeAsync(Guid id, BackflushConsumeCommandDto input);
}
