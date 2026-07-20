using Microsoft.EntityFrameworkCore;
using Wms.DataDictionary.Domain.Entities;

namespace Wms.DataDictionary.EntityFrameworkCore;

public class WmsDataDictionaryDbContext : DbContext
{
    public DbSet<Dictionary> DataDictionaries { get; set; }
    public DbSet<DataDictionaryItem> DataDictionaryItems { get; set; }

    public WmsDataDictionaryDbContext(DbContextOptions<WmsDataDictionaryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(WmsDataDictionaryDbContext).Assembly);
    }
}
