using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Swashbuckle;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.HttpApi;
using Wms.Shared;
using Wms.Warehouse.HttpApi;
using Wms.Warehouse.Application;
using Wms.Warehouse.EntityFrameworkCore;
using Wms.Material.HttpApi;
using Wms.Material.Application;
using Wms.Material.EntityFrameworkCore;
using Wms.Inventory.HttpApi;
using Wms.Inventory.Application;
using Wms.Inventory.EntityFrameworkCore;
using Wms.Inbound.HttpApi;
using Wms.Inbound.Application;
using Wms.Inbound.EntityFrameworkCore;
using Wms.Outbound.HttpApi;
using Wms.Outbound.Application;
using Wms.Outbound.EntityFrameworkCore;
using Wms.Transfer.HttpApi;
using Wms.Transfer.Application;
using Wms.Transfer.EntityFrameworkCore;
using Wms.CycleCount.HttpApi;
using Wms.CycleCount.Application;
using Wms.CycleCount.EntityFrameworkCore;
using Wms.LineSide.HttpApi;
using Wms.LineSide.Application;
using Wms.LineSide.EntityFrameworkCore;
using Wms.Production.HttpApi;
using Wms.Production.Application;
using Wms.Production.EntityFrameworkCore;
using Wms.TaskCenter.HttpApi;
using Wms.TaskCenter.Application;
using Wms.TaskCenter.EntityFrameworkCore;
using Wms.BarcodeLabel.HttpApi;
using Wms.BarcodeLabel.Application;
using Wms.BarcodeLabel.EntityFrameworkCore;
using Wms.Workflow.HttpApi;
using Wms.Workflow.Application;
using Wms.Workflow.EntityFrameworkCore;
using Wms.RuleEngine.HttpApi;
using Wms.RuleEngine.Application;
using Wms.RuleEngine.EntityFrameworkCore;
using Wms.Notification.HttpApi;
using Wms.Notification.Application;
using Wms.Notification.EntityFrameworkCore;
using Wms.Supplier.HttpApi;
using Wms.Supplier.Application;
using Wms.Supplier.EntityFrameworkCore;

namespace Wms.HttpApi.Host;

[DependsOn(
    typeof(WmsSharedModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(WmsWarehouseHttpApiModule),
    typeof(WmsWarehouseApplicationModule),
    typeof(WmsWarehouseEntityFrameworkCoreModule),
    typeof(WmsMaterialHttpApiModule),
    typeof(WmsMaterialApplicationModule),
    typeof(WmsMaterialEntityFrameworkCoreModule),
    typeof(WmsInventoryHttpApiModule),
    typeof(WmsInventoryApplicationModule),
    typeof(WmsInventoryEntityFrameworkCoreModule),
    typeof(WmsInboundHttpApiModule),
    typeof(WmsInboundApplicationModule),
    typeof(WmsInboundEntityFrameworkCoreModule),
    typeof(WmsOutboundHttpApiModule),
    typeof(WmsOutboundApplicationModule),
    typeof(WmsOutboundEntityFrameworkCoreModule),
    typeof(WmsTransferHttpApiModule),
    typeof(WmsTransferApplicationModule),
    typeof(WmsTransferEntityFrameworkCoreModule),
    typeof(WmsCycleCountHttpApiModule),
    typeof(WmsCycleCountApplicationModule),
    typeof(WmsCycleCountEntityFrameworkCoreModule),
    typeof(WmsLineSideHttpApiModule),
    typeof(WmsLineSideApplicationModule),
    typeof(WmsLineSideEntityFrameworkCoreModule),
    typeof(WmsProductionHttpApiModule),
    typeof(WmsProductionApplicationModule),
    typeof(WmsProductionEntityFrameworkCoreModule),
    typeof(WmsTaskCenterHttpApiModule),
    typeof(WmsTaskCenterApplicationModule),
    typeof(WmsTaskCenterEntityFrameworkCoreModule),
    typeof(WmsBarcodeLabelHttpApiModule),
    typeof(WmsBarcodeLabelApplicationModule),
    typeof(WmsBarcodeLabelEntityFrameworkCoreModule),
    typeof(WmsWorkflowHttpApiModule),
    typeof(WmsWorkflowApplicationModule),
    typeof(WmsWorkflowEntityFrameworkCoreModule),
    typeof(WmsRuleEngineHttpApiModule),
    typeof(WmsRuleEngineApplicationModule),
    typeof(WmsRuleEngineEntityFrameworkCoreModule),
    typeof(WmsNotificationHttpApiModule),
    typeof(WmsNotificationApplicationModule),
    typeof(WmsNotificationEntityFrameworkCoreModule),
    typeof(WmsSupplierHttpApiModule),
    typeof(WmsSupplierApplicationModule),
    typeof(WmsSupplierEntityFrameworkCoreModule)
)]
public class WmsHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();


        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer();
        });

        // Configure Swagger with DocInclusionPredicate that includes
        // ABP module auto-controllers regardless of GroupName
        context.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("WMS", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "WMS API",
                Version = "v1",
                Description = "Manufacturing WMS Backend API"
            });
            options.DocInclusionPredicate((docName, api) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
        // AbpSwaggerOptions is commercial-only; removed in open-source migration
        // Configure CORS
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(configuration["App:CorsOrigins"] ?? "http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Configure SignalR
        context.Services.AddSignalR();

    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment() ?? app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
        app.UseCors();
        app.UseSwagger();
        // Swagger only in non-production environments
        if (!env.IsProduction())
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/WMS/swagger.json", "WMS API");
            });
        }
        app.UseAbpRequestLocalization();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<WmsNotificationHub>("/signalr/notification");
        });
    }
}
