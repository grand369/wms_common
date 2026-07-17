using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;

namespace Wms.DbMigrator;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/migrator.txt", rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Log.Information("Starting WMS DbMigrator...");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            using (var application = await AbpApplicationFactory.CreateAsync<WmsDbMigratorModule>(options =>
            {
                options.UseAutofac();
                options.Services.AddLogging(c => c.AddSerilog());
                options.Services.AddSingleton<IConfiguration>(configuration);
            }))
            {
                await application.InitializeAsync();

                Log.Information("Seeding initial data...");
                await application.ServiceProvider.GetRequiredService<IDataSeeder>().SeedAsync(new DataSeedContext());

                await application.ShutdownAsync();
            }

            Log.Information("WMS DbMigrator completed successfully!");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "WMS DbMigrator terminated unexpectedly!");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}