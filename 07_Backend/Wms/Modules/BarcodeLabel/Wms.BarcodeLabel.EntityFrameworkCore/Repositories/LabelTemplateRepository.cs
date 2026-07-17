using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.BarcodeLabel.Domain.Aggregates;
using Wms.BarcodeLabel.Domain.Enums;
using Wms.BarcodeLabel.Domain.Repositories;

namespace Wms.BarcodeLabel.EntityFrameworkCore.Repositories;

public class LabelTemplateRepository : EfCoreRepository<WmsBarcodeLabelDbContext, LabelTemplate, Guid>, ILabelTemplateRepository
{
    public LabelTemplateRepository(IDbContextProvider<WmsBarcodeLabelDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<LabelTemplate?> FindByTemplateNameAsync(string templateName)
        => await (await GetDbSetAsync()).FirstOrDefaultAsync(t => t.TemplateName == templateName);

    public async Task<List<LabelTemplate>> GetByTemplateTypeAsync(LabelTemplateType templateType)
        => await (await GetDbSetAsync()).Where(t => t.TemplateType == templateType).OrderBy(t => t.TemplateName).ToListAsync();

    public async Task<List<LabelTemplate>> GetActiveTemplatesAsync()
        => await (await GetDbSetAsync()).Where(t => t.IsActive).OrderBy(t => t.TemplateName).ToListAsync();
}
