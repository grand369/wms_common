using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Volo.Abp.ObjectExtending;

namespace Wms.EntityFrameworkCore.SqlServer;

public class WmsDbContextFactory : IDesignTimeDbContextFactory<WmsDbContext>
{
    public WmsDbContext CreateDbContext(string[] args)
    {
        ObjectExtensionManager.Instance
            .AddOrUpdate<object>(options => { });

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<WmsDbContext>();
        builder.UseSqlServer(configuration.GetConnectionString("Default"),
            optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(WmsDbContextFactory).Assembly.GetName().Name));

        return new WmsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var basePath = Directory.GetCurrentDirectory();

        var appSettingsPath = Path.Combine(basePath, "appsettings.json");
        if (!File.Exists(appSettingsPath))
        {
            basePath = Path.Combine(basePath, "Host", "Wms.Web.Host");
        }

        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }
}