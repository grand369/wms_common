using Wms.Inventory.Application.Contracts.Dtos;
using Wms.Inventory.Application.Contracts.Services;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Wms.Inventory.Application.Services;

/// <summary>
/// Inventory Snapshot App Service — provides snapshot management functionality.
/// </summary>
public class InventorySnapshotAppService : ApplicationService, IInventorySnapshotAppService
{
    private readonly IInventorySnapshotRepository _snapshotRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;

    public InventorySnapshotAppService(
        IInventorySnapshotRepository snapshotRepository,
        IInventoryBalanceRepository balanceRepository)
    {
        _snapshotRepository = snapshotRepository;
        _balanceRepository = balanceRepository;
    }

    public async Task<PagedResultDto<InventorySnapshotOutputDto>> GetListAsync(InventorySnapshotQueryDto query)
    {
        var queryable = await _snapshotRepository.GetQueryableAsync();

        if (query.WarehouseId.HasValue)
            queryable = queryable.Where(s => s.WarehouseId == query.WarehouseId.Value);
        if (query.Status.HasValue)
            queryable = queryable.Where(s => s.Status == query.Status.Value);
        if (!string.IsNullOrEmpty(query.Keyword))
            queryable = queryable.Where(s =>
                s.SnapshotNo.Contains(query.Keyword) ||
                s.WarehouseCode.Contains(query.Keyword));

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.OrderByDescending(s => s.SnapshotTime)
                .Skip(query.SkipCount).Take(query.MaxResultCount));

        return new PagedResultDto<InventorySnapshotOutputDto>(totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    public async Task<InventorySnapshotOutputDto> GetAsync(Guid id)
    {
        var snapshot = await _snapshotRepository.GetAsync(id);
        return MapToOutputDto(snapshot);
    }

    public async Task<InventorySnapshotOutputDto> CreateAsync(InventorySnapshotCreateDto dto)
    {
        // Get warehouse name/info from balance
        var warehouseBalances = await _balanceRepository.GetByWarehouseAsync(dto.WarehouseId);
        
        var totalQty = warehouseBalances.Sum(b => b.Quantity);
        var totalFrozenQty = warehouseBalances.Sum(b => b.FrozenQuantity);
        var totalAvailableQty = warehouseBalances.Sum(b => b.AvailableQuantity);
        var warehouseCode = warehouseBalances.FirstOrDefault()?.WarehouseCode ?? string.Empty;

        var snapshot = new InventorySnapshot(
            GuidGenerator.Create(),
            GenerateSnapshotNo(),
            dto.WarehouseId,
            warehouseCode,
            DateTime.UtcNow,
            totalQty,
            totalFrozenQty,
            totalAvailableQty,
            dto.Remark);

        await _snapshotRepository.InsertAsync(snapshot);

        return MapToOutputDto(snapshot);
    }

    private InventorySnapshotOutputDto MapToOutputDto(InventorySnapshot snapshot)
    {
        return new InventorySnapshotOutputDto
        {
            Id = snapshot.Id,
            SnapshotNo = snapshot.SnapshotNo,
            WarehouseId = snapshot.WarehouseId,
            WarehouseCode = snapshot.WarehouseCode,
            SnapshotTime = snapshot.SnapshotTime,
            TotalQty = snapshot.TotalQty,
            TotalFrozenQty = snapshot.TotalFrozenQty,
            TotalAvailableQty = snapshot.TotalAvailableQty,
            Status = snapshot.Status,
            Remark = snapshot.Remark,
            CreationTime = snapshot.CreationTime
        };
    }

    private static string GenerateSnapshotNo()
    {
        return $"SNAP-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
    }
}
