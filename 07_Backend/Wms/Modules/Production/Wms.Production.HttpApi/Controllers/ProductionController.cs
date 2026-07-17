using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Production.Application.Contracts.Dtos;
using Wms.Production.Application.Contracts.Services;

namespace Wms.Production.HttpApi.Controllers;

[RemoteService(Name = "WmsProduction")]
[Area("WmsProduction")]
[Route("api/v1/production")]
[Authorize]
public class ProductionController : AbpControllerBase
{
    private readonly IProductionAppService _appService;
    public ProductionController(IProductionAppService appService) => _appService = appService;

    [HttpGet("orders")] public Task<PagedResultDto<ProductionOrderOutputDto>> GetOrdersAsync(ProductionOrderQueryDto query) => _appService.GetOrdersAsync(query);
    [HttpGet("orders/{id}")] public Task<ProductionOrderOutputDto> GetOrderAsync(Guid id) => _appService.GetOrderAsync(id);
    [HttpPost("orders")] public Task<ProductionOrderOutputDto> CreateOrderAsync(ProductionOrderCreateDto input) => _appService.CreateOrderAsync(input);
    [HttpGet("requisitions")] public Task<PagedResultDto<MaterialRequisitionOutputDto>> GetRequisitionsAsync(MaterialRequisitionQueryDto query) => _appService.GetRequisitionsAsync(query);
    [HttpGet("requisitions/{id}")] public Task<MaterialRequisitionOutputDto> GetRequisitionAsync(Guid id) => _appService.GetRequisitionAsync(id);
    [HttpPost("requisitions/generate-from-order/{orderId}")] public Task<MaterialRequisitionOutputDto> GenerateRequisitionFromOrderAsync(Guid orderId) => _appService.GenerateRequisitionFromOrderAsync(orderId);
    [HttpPatch("orders/{id}/complete-production")] public Task<ProductionOrderOutputDto> CompleteProductionAsync(Guid id) => _appService.CompleteProductionAsync(id);
}
