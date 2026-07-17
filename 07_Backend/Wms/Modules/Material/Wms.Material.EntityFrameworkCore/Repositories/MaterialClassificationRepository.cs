using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Material.Domain.Aggregates;
using Wms.Material.Domain.Repositories;

namespace Wms.Material.EntityFrameworkCore.Repositories;

public class MaterialClassificationRepository : EfCoreRepository<WmsMaterialDbContext, MaterialClassification, Guid>, IMaterialClassificationRepository
{
    public MaterialClassificationRepository(IDbContextProvider<WmsMaterialDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<MaterialClassification?> FindByCodeAsync(string classificationCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.MaterialClassifications.FirstOrDefaultAsync(c => c.ClassificationCode == classificationCode);
    }

    public async Task<List<MaterialClassification>> GetTreeAsync()
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.MaterialClassifications.OrderBy(c => c.ClassificationLevel).ThenBy(c => c.ClassificationCode).ToListAsync();
    }

    public async Task<List<MaterialClassification>> GetListByParentIdAsync(Guid? parentId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.MaterialClassifications.Where(c => c.ParentClassificationId == parentId).OrderBy(c => c.ClassificationCode).ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string classificationCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.MaterialClassifications.AnyAsync(c => c.ClassificationCode == classificationCode);
    }
}
