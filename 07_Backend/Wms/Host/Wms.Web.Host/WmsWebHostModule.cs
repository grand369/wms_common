using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.Swashbuckle;
using Wms.BarcodeLabel.Application;
using Wms.BarcodeLabel.EntityFrameworkCore;
using Wms.BarcodeLabel.HttpApi;
using Wms.CycleCount.Application;
using Wms.CycleCount.EntityFrameworkCore;
using Wms.CycleCount.HttpApi;
using Wms.EntityFrameworkCore.SqlServer;
using Wms.Inbound.Application;
using Wms.Inbound.EntityFrameworkCore;
using Wms.Inbound.HttpApi;
using Wms.Inventory.Application;
using Wms.Inventory.EntityFrameworkCore;
using Wms.Inventory.HttpApi;
using Wms.LineSide.Application;
using Wms.LineSide.EntityFrameworkCore;
using Wms.LineSide.HttpApi;
using Wms.Material.Application;
using Wms.Material.EntityFrameworkCore;
using Wms.Material.HttpApi;
using Wms.Notification.Application;
using Wms.Notification.EntityFrameworkCore;
using Wms.Notification.HttpApi;
using Wms.Outbound.Application;
using Wms.Outbound.EntityFrameworkCore;
using Wms.Outbound.HttpApi;
using Wms.Production.Application;
using Wms.Production.EntityFrameworkCore;
using Wms.Production.HttpApi;
using Wms.RuleEngine.Application;
using Wms.RuleEngine.EntityFrameworkCore;
using Wms.RuleEngine.HttpApi;
using Wms.Shared;
using Wms.TaskCenter.Application;
using Wms.TaskCenter.EntityFrameworkCore;
using Wms.TaskCenter.HttpApi;
using Wms.Transfer.Application;
using Wms.Transfer.EntityFrameworkCore;
using Wms.Transfer.HttpApi;
using Wms.Warehouse.Application;
using Wms.Warehouse.EntityFrameworkCore;
using Wms.Warehouse.HttpApi;
using Wms.Web.Host.Auth;
using Wms.Workflow.Application;
using Wms.Workflow.EntityFrameworkCore;
using Wms.Workflow.HttpApi;
using Microsoft.Extensions.DependencyInjection;
namespace Wms.Web.Host;

[DependsOn(
    typeof(WmsSharedModule),
    typeof(WmsEntityFrameworkCoreSqlServerModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAspNetCoreMvcUiModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
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
    typeof(WmsNotificationEntityFrameworkCoreModule)
)]
public class WmsWebHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        var defaultConnStr = configuration.GetConnectionString("Default");
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = defaultConnStr;
            options.ConnectionStrings["AbpIdentity"] = defaultConnStr;
        });

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = false;
        });

        // ABP PermissionManagement requires explicit provider policy mapping.
        // "R" = Role-level permissions → requires AbpIdentity.Roles permission
        // "U" = User-level permissions → requires AbpIdentity.Users permission
        // Without this, GET/PUT /api/permission-management/permissions throws:
        //   "No policy defined to get/set permissions for the provider 'R'"
        Configure<PermissionManagementOptions>(options =>
        {
            // 1. 配置 Provider 的授权策略（GET/PUT 请求都需要）
            options.ProviderPolicies["R"] = "AbpIdentity.Roles";
            options.ProviderPolicies["U"] = "AbpIdentity.Users";

            // 2. 注册 Management Providers（PUT/SetAsync 操作需要）
            if (!options.ManagementProviders.Contains<RolePermissionManagementProvider>())
            {
                options.ManagementProviders.Add<RolePermissionManagementProvider>();
            }

            if (!options.ManagementProviders.Contains<UserPermissionManagementProvider>())
            {
                options.ManagementProviders.Add<UserPermissionManagementProvider>();
            }
        });

        // Register IPermissionManagementProvider implementations.
        // ABP's AbpPermissionManagementModule registers these, but if the module
        // initialization order skips them, PermissionManager.SetAsync throws:
        //   "Unknown permission management provider: R"
        // Using TryAddEnumerable prevents duplicate registrations.
        context.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPermissionManagementProvider, RolePermissionManagementProvider>());
        context.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPermissionManagementProvider, UserPermissionManagementProvider>());

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
        });

        var jwtSecret = configuration["Jwt:Secret"] ?? "Wms-Super-Secret-Key-2025-Must-Be-256-Bits!";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? configuration["App:SelfUrl"] ?? "https://localhost:5000";
        var jwtAudience = configuration["Jwt:Audience"] ?? "WmsApi";

        context.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                NameClaimType = "unique_name",
                RoleClaimType = ClaimTypes.Role
            };
        });

        Configure<JwtBearerOptions>("Bearer", options =>
        {
            options.MapInboundClaims = false;
        });

        context.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("WMS", new OpenApiInfo
            {
                Title = "WMS API",
                Version = "v1",
                Description = "Manufacturing WMS Backend API"
            });

            // ABP auto-generated controllers (e.g. Identity) set a GroupName
            // based on module name (e.g. "AbpIdentity") which does not match
            // "WMS".  The default Swashbuckle predicate would exclude them.
            // This override includes EVERY endpoint so all ABP module APIs
            // (Identity, PermissionManagement, …) show up in the WMS doc.
            options.DocInclusionPredicate((docName, api) => true);

            options.CustomSchemaIds(type => type.FullName);
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Register Claims-based permission handler BEFORE ABP's database-based handler.
        // If claims contain the required permission, this handler succeeds the requirement
        // immediately; otherwise it does nothing and ABP's AbpPermissionAuthorizationHandler
        // (database lookup) continues as fallback.
        context.Services.AddTransient<IAuthorizationHandler, ClaimsPermissionAuthorizationHandler>();

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(configuration["App:CorsOrigins"]!
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.RemovePostFix("/"))
                    .ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        context.Services.AddSignalR();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment() ?? app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
        app.UseRouting();

        app.UseCors();
        app.UseSwagger();
        if (!env.IsProduction())
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/WMS/swagger.json", "WMS API");
            });
        }
        var supportedCultures = new[] { "zh-Hans", "en" };
        app.UseRequestLocalization(new Microsoft.AspNetCore.Builder.RequestLocalizationOptions
        {
            DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("zh-Hans"),
            SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList(),
            SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList()
        });
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