using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Wms.Supplier.Application.Contracts.Dtos;
using Wms.Supplier.Application.Contracts.Permissions;
using Wms.Supplier.Application.Contracts.Services;
using SupplierAgg = Wms.Supplier.Domain.Aggregates.Supplier;

namespace Wms.Supplier.Application.Services;

/// <summary>
/// Supplier App Service Implementation — implements CRUD operations for Supplier aggregate.
/// </summary>
[Authorize(WmsSupplierPermissions.Suppliers.Default)]

public class SupplierAppService : ApplicationService, ISupplierAppService
{
    private readonly IRepository<SupplierAgg, Guid> _supplierRepository;

    public SupplierAppService(IRepository<SupplierAgg, Guid> supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    [Authorize(WmsSupplierPermissions.Suppliers.View)]
    public async Task<SupplierOutputDto> GetAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetAsync(id);
        return ObjectMapper.Map<SupplierAgg, SupplierOutputDto>(supplier);
    }

    [Authorize(WmsSupplierPermissions.Suppliers.View)]
    public async Task<SupplierOutputDto> GetByCodeAsync(string supplierCode)
    {
        var supplier = await _supplierRepository.FirstOrDefaultAsync(s => s.SupplierCode == supplierCode);
        if (supplier == null)
        {
            throw new EntityNotFoundException(typeof(SupplierAgg), supplierCode);
        }
        return ObjectMapper.Map<SupplierAgg, SupplierOutputDto>(supplier);
    }

    [Authorize(WmsSupplierPermissions.Suppliers.List)]
    public async Task<PagedResultDto<SupplierOutputDto>> GetListAsync(SupplierQueryDto query)
    {
        var queryable = await _supplierRepository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(query.SupplierCode), s => s.SupplierCode.Contains(query.SupplierCode!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.SupplierName), s => s.SupplierName.Contains(query.SupplierName!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Filter), s => s.SupplierCode.Contains(query.Filter!) || s.SupplierName.Contains(query.Filter!))
            .WhereIf(query.SupplierType.HasValue, s => s.SupplierType == query.SupplierType.Value)
            .WhereIf(query.IsActive.HasValue, s => s.IsActive == query.IsActive.Value);

        var totalCount = await AsyncExecuter.LongCountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(s => s.CreationTime).Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<SupplierOutputDto>(totalCount, ObjectMapper.Map<List<SupplierAgg>, List<SupplierOutputDto>>(items));
    }

    [Authorize(WmsSupplierPermissions.Suppliers.Create)]
    public async Task<SupplierOutputDto> CreateAsync(SupplierCreateDto input)
    {
        if (await _supplierRepository.AnyAsync(s => s.SupplierCode == input.SupplierCode))
        {
            throw new BusinessException("SupplierCodeAlreadyExists")
                .WithData("SupplierCode", input.SupplierCode);
        }

        var supplier = new SupplierAgg(GuidGenerator.Create(), input.SupplierCode, input.SupplierName);

        supplier.Update(
            input.SupplierName,
            input.ShortName,
            input.SupplierType,
            input.ContactName,
            input.ContactPhone,
            input.ContactEmail,
            input.Address,
            input.City,
            input.Province,
            input.PostalCode,
            input.TaxId,
            input.BankName,
            input.BankAccount,
            input.IsActive,
            input.Remark,
            input.ErpSupplierCode
        );

        await _supplierRepository.InsertAsync(supplier, autoSave: true);

        return ObjectMapper.Map<SupplierAgg, SupplierOutputDto>(supplier);
    }

    [Authorize(WmsSupplierPermissions.Suppliers.Update)]
    public async Task<SupplierOutputDto> UpdateAsync(Guid id, SupplierUpdateDto input)
    {
        var supplier = await _supplierRepository.GetAsync(id);

        supplier.Update(
            input.SupplierName,
            input.ShortName,
            input.SupplierType,
            input.ContactName,
            input.ContactPhone,
            input.ContactEmail,
            input.Address,
            input.City,
            input.Province,
            input.PostalCode,
            input.TaxId,
            input.BankName,
            input.BankAccount,
            input.IsActive,
            input.Remark,
            input.ErpSupplierCode
        );

        await _supplierRepository.UpdateAsync(supplier, autoSave: true);

        return ObjectMapper.Map<SupplierAgg, SupplierOutputDto>(supplier);
    }

    [Authorize(WmsSupplierPermissions.Suppliers.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _supplierRepository.DeleteAsync(id, autoSave: true);
    }

    [Authorize(WmsSupplierPermissions.Suppliers.Activate)]
    public async Task<SupplierOutputDto> ActivateAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetAsync(id);
        supplier.Activate();
        await _supplierRepository.UpdateAsync(supplier, autoSave: true);
        return ObjectMapper.Map<SupplierAgg, SupplierOutputDto>(supplier);
    }

    [Authorize(WmsSupplierPermissions.Suppliers.Deactivate)]
    public async Task<SupplierOutputDto> DeactivateAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetAsync(id);
        supplier.Deactivate();
        await _supplierRepository.UpdateAsync(supplier, autoSave: true);
        return ObjectMapper.Map<SupplierAgg, SupplierOutputDto>(supplier);
    }

    [AllowAnonymous]
    public async Task<List<SupplierOutputDto>> GetActiveSuppliersAsync()
    {
        var suppliers = await _supplierRepository.GetListAsync(s => s.IsActive);
        return ObjectMapper.Map<List<SupplierAgg>, List<SupplierOutputDto>>(suppliers);
    }
}
