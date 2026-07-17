using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Repositories;

/// <summary>
/// IBarcodeRuleRepository (REP-17) — custom query methods for BarcodeRule aggregate.
/// </summary>
public interface IBarcodeRuleRepository : IRepository<BarcodeRule, Guid>
{
    /// <summary>Find barcode rule by rule name (unique business key).</summary>
    Task<BarcodeRule?> FindByRuleNameAsync(string ruleName);

    /// <summary>Get barcode rules by barcode type.</summary>
    Task<List<BarcodeRule>> GetByBarcodeTypeAsync(BarcodeType barcodeType);

    /// <summary>Get all active barcode rules.</summary>
    Task<List<BarcodeRule>> GetActiveRulesAsync();
}
