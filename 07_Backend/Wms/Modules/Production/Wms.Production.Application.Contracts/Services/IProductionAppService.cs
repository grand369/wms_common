using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Wms.Production.Application.Contracts.Dtos;

namespace Wms.Production.Application.Contracts.Services;

/// <summary>IProductionAppService — 7 API methods</summary>
public interface IProductionAppService : IApplicationService
{
    Task<PagedResultDto<ProductionOrderOutputDto>> GetOrdersAsync(ProductionOrderQueryDto query);
    Task<ProductionOrderOutputDto> GetOrderAsync(Guid id);
    Task<ProductionOrderOutputDto> CreateOrderAsync(ProductionOrderCreateDto input);
    Task<PagedResultDto<MaterialRequisitionOutputDto>> GetRequisitionsAsync(MaterialRequisitionQueryDto query);
    Task<MaterialRequisitionOutputDto> GetRequisitionAsync(Guid id);
    Task<MaterialRequisitionOutputDto> GenerateRequisitionFromOrderAsync(Guid orderId);
    Task<ProductionOrderOutputDto> CompleteProductionAsync(Guid id);
}
