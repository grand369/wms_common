using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Supplier.Application.Contracts.Dtos;
using Wms.Supplier.Application.Contracts.Services;
using Volo.Abp.AspNetCore.Mvc;

namespace Wms.Supplier.HttpApi.Controllers;

/// <summary>
/// Supplier Controller — API endpoints for supplier operations.
/// Route: /api/v1/supplier/suppliers
/// </summary>
[Route("api/v1/supplier/suppliers")]
[Authorize]
public class SupplierController : AbpControllerBase
{
    private readonly ISupplierAppService _appService;

    public SupplierController(ISupplierAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<PagedResultDto<SupplierOutputDto>> GetListAsync([FromQuery] SupplierQueryDto query)
    {
        return await _appService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    public async Task<SupplierOutputDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpGet("by-code/{supplierCode}")]
    public async Task<SupplierOutputDto> GetByCodeAsync(string supplierCode)
    {
        return await _appService.GetByCodeAsync(supplierCode);
    }

    [HttpGet("active")]
    public async Task<List<SupplierOutputDto>> GetActiveSuppliersAsync()
    {
        return await _appService.GetActiveSuppliersAsync();
    }

    [HttpPost]
    public async Task<SupplierOutputDto> CreateAsync([FromBody] SupplierCreateDto dto)
    {
        return await _appService.CreateAsync(dto);
    }

    [HttpPut("{id}")]
    public async Task<SupplierOutputDto> UpdateAsync(Guid id, [FromBody] SupplierUpdateDto dto)
    {
        return await _appService.UpdateAsync(id, dto);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }

    [HttpPatch("{id}/activate")]
    public async Task<SupplierOutputDto> ActivateAsync(Guid id)
    {
        return await _appService.ActivateAsync(id);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<SupplierOutputDto> DeactivateAsync(Guid id)
    {
        return await _appService.DeactivateAsync(id);
    }
}
