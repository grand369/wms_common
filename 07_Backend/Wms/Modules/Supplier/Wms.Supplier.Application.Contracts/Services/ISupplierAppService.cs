using Wms.Supplier.Application.Contracts.Dtos;

namespace Wms.Supplier.Application.Contracts.Services;

/// <summary>
/// Supplier App Service Interface — defines CRUD operations for Supplier aggregate.
/// </summary>
public interface ISupplierAppService
{
    Task<SupplierOutputDto> GetAsync(Guid id);
    
    Task<SupplierOutputDto> GetByCodeAsync(string supplierCode);
    
    Task<PagedResultDto<SupplierOutputDto>> GetListAsync(SupplierQueryDto query);
    
    Task<SupplierOutputDto> CreateAsync(SupplierCreateDto input);
    
    Task<SupplierOutputDto> UpdateAsync(Guid id, SupplierUpdateDto input);
    
    Task DeleteAsync(Guid id);
    
    Task<SupplierOutputDto> ActivateAsync(Guid id);
    
    Task<SupplierOutputDto> DeactivateAsync(Guid id);
    
    Task<List<SupplierOutputDto>> GetActiveSuppliersAsync();
}
