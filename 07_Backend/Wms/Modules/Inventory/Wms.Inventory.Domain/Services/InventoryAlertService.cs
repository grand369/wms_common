using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace Wms.Inventory.Domain.Services;

/// <summary>
/// Inventory Alert Service — scans for inventory anomalies and creates alerts.
/// Called by scheduled tasks (background jobs).
/// </summary>
public class InventoryAlertService : DomainService
{
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly IInventoryAlertRepository _alertRepository;
    private readonly ILocalEventBus _localEventBus;

    public InventoryAlertService(
        IInventoryBalanceRepository balanceRepository,
        IInventoryAlertRepository alertRepository,
        ILocalEventBus localEventBus)
    {
        _balanceRepository = balanceRepository;
        _alertRepository = alertRepository;
        _localEventBus = localEventBus;
    }

    /// <summary>Scan for zero inventory and create alerts.</summary>
    public async Task ScanZeroInventoryAsync()
    {
        var zeroBalances = await _balanceRepository.GetZeroInventoryAsync();
        foreach (var balance in zeroBalances)
        {
            var alert = new InventoryAlert(
                GuidGenerator.Create(),
                AlertType.ZeroInventory,
                balance.MaterialId,
                balance.MaterialCode,
                balance.WarehouseId,
                balance.WarehouseCode,
                balance.Quantity,
                0m);
            await _alertRepository.InsertAsync(alert);
        }
    }

    /// <summary>Scan for near-expiry inventory and create alerts.</summary>
    public async Task ScanExpiryAsync(int alertDays = 30)
    {
        var nearExpiry = await _balanceRepository.GetNearExpiryAsync(alertDays);
        foreach (var balance in nearExpiry)
        {
            var alert = new InventoryAlert(
                GuidGenerator.Create(),
                AlertType.Expiry,
                balance.MaterialId,
                balance.MaterialCode,
                balance.WarehouseId,
                balance.WarehouseCode,
                balance.Quantity,
                0m);
            await _alertRepository.InsertAsync(alert);
        }
    }

    /// <summary>Scan for safety stock breaches and create alerts.</summary>
    public async Task ScanSafetyStockAsync()
    {
        var belowSafetyStock = await _balanceRepository.GetBelowSafetyStockAsync();
        foreach (var balance in belowSafetyStock)
        {
            var alert = new InventoryAlert(
                GuidGenerator.Create(),
                AlertType.SafetyStock,
                balance.MaterialId,
                balance.MaterialCode,
                balance.WarehouseId,
                balance.WarehouseCode,
                balance.AvailableQuantity,
                0m); // SafetyStock threshold would come from Material module
            await _alertRepository.InsertAsync(alert);
        }
    }
}
