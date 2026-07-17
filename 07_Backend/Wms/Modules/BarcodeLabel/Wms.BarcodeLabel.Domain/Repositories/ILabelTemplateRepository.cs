using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;

namespace Wms.BarcodeLabel.Domain.Repositories;

/// <summary>
/// ILabelTemplateRepository (REP-18) — custom query methods for LabelTemplate aggregate.
/// </summary>
public interface ILabelTemplateRepository : IRepository<LabelTemplate, Guid>
{
    /// <summary>Find label template by template name (unique business key).</summary>
    Task<LabelTemplate?> FindByTemplateNameAsync(string templateName);

    /// <summary>Get label templates by template type.</summary>
    Task<List<LabelTemplate>> GetByTemplateTypeAsync(LabelTemplateType templateType);

    /// <summary>Get all active label templates.</summary>
    Task<List<LabelTemplate>> GetActiveTemplatesAsync();
}
