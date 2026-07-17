using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using MaterialAgg = Wms.Material.Domain.Aggregates.Material;
using Wms.Material.Domain.Repositories;

namespace Wms.Material.EntityFrameworkCore.Repositories;

public class MaterialRepository : EfCoreRepository<WmsMaterialDbContext, MaterialAgg, Guid>, IMaterialRepository
{
    public MaterialRepository(IDbContextProvider<WmsMaterialDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<MaterialAgg?> FindByCodeAsync(string materialCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Materials.Include(m => m.SubstituteRelations)
            .FirstOrDefaultAsync(m => m.MaterialCode == materialCode);
    }

    public async Task<List<MaterialAgg>> GetListByClassificationIdAsync(Guid classificationId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Materials.Include(m => m.SubstituteRelations)
            .Where(m => m.ClassificationId == classificationId).OrderBy(m => m.MaterialCode).ToListAsync();
    }

    public async Task<List<MaterialAgg>> GetListByTypeAsync(int materialType)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Materials.Include(m => m.SubstituteRelations)
            .Where(m => m.MaterialType == materialType).OrderBy(m => m.MaterialCode).ToListAsync();
    }

    public async Task<List<MaterialAgg>> GetActiveListAsync()
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Materials.Include(m => m.SubstituteRelations)
            .Where(m => m.IsActive).OrderBy(m => m.MaterialCode).ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string materialCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Materials.AnyAsync(m => m.MaterialCode == materialCode);
    }
}
