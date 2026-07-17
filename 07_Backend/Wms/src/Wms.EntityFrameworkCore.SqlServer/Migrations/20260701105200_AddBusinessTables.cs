using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ImpersonatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImpersonatorUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ImpersonatorTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImpersonatorTenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "int", nullable: false),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Exceptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpBackgroundJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    JobArgs = table.Column<string>(type: "nvarchar(max)", maxLength: 1048576, nullable: false),
                    TryCount = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextTryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAbandoned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Priority = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)15),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBackgroundJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpClaimTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    Regex = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RegexDescription = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpClaimTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatureGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "bit", nullable: false),
                    IsAvailableToHost = table.Column<bool>(type: "bit", nullable: false),
                    AllowedProviders = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatureValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpLinkUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpLinkUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(95)", maxLength: 95, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EntityVersion = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGrants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGrants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    MultiTenancySide = table.Column<byte>(type: "tinyint", nullable: false),
                    Providers = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StateCheckers = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsStatic = table.Column<bool>(type: "bit", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    EntityVersion = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSecurityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    Identity = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSecurityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Device = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IpAddresses = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SignedIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettingDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "bit", nullable: false),
                    Providers = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    IsInherited = table.Column<bool>(type: "bit", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettingDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ProviderKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserDelegations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserDelegations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ShouldChangePasswordOnNextLogin = table.Column<bool>(type: "bit", nullable: false),
                    EntityVersion = table.Column<int>(type: "int", nullable: false),
                    LastPasswordChangeTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_CycleCount_CycleCountPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountMethod = table.Column<int>(type: "int", nullable: false),
                    CountStatus = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FreezeInventory = table.Column<bool>(type: "bit", nullable: false),
                    DifferenceThreshold = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    BlindCountEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_CycleCount_CycleCountPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_CycleCount_CycleCountResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    DifferenceQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    DifferenceAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    BlindCountFlag = table.Column<bool>(type: "bit", nullable: false),
                    ResultStatusValue = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_CycleCount_CycleCountResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Inbound_InboundOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InboundOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InboundType = table.Column<int>(type: "int", nullable: false),
                    InboundStatus = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductionOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReturnOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OverReceiptRatio = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    QualityInspectionRequired = table.Column<bool>(type: "bit", nullable: false),
                    TotalPlanQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalReceivedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErpCallbackStatus = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Inbound_InboundOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Inventory_InventoryAdjustment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdjustmentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AdjustmentType = table.Column<int>(type: "int", nullable: false),
                    AdjustmentReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Inventory_InventoryAdjustment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Inventory_InventoryAlert",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlertType = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ThresholdQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    AlertTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolveTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Inventory_InventoryAlert", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Inventory_InventoryBalance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InventoryStatus = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    FrozenQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InTransitQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    LastOperationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConcurrencyVersion = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Inventory_InventoryBalance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Inventory_InventoryFreezeOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FreezeOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FreezeScope = table.Column<int>(type: "int", nullable: false),
                    FreezeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FreezeStatus = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    FreezeStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FreezeEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Inventory_InventoryFreezeOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Inventory_InventoryLedgerEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryBalanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    OperationQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BeforeQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AfterQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BeforeAvailable = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AfterAvailable = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    OperationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SourceOrderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Inventory_InventoryLedgerEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_LineSide_LineSideWarehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineSideWarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LineSideWarehouseName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductionLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionLineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkStationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConsumptionMode = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_LineSide_LineSideWarehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Material_Material",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaterialNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ClassificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Specification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrimaryUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryUnitName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SecondaryUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConversionRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    MaterialType = table.Column<int>(type: "int", nullable: false),
                    StorageAttribute = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QualityAttribute = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InventoryAttribute = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueStrategy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DangerAttribute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ErpSyncStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Material_Material", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Material_MaterialClassification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassificationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClassificationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParentClassificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClassificationLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AttributeTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Material_MaterialClassification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Material_UnitOfMeasure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitSymbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UnitType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Material_UnitOfMeasure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Outbound_OutboundOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutboundOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OutboundType = table.Column<int>(type: "int", nullable: false),
                    OutboundStatus = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialRequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SalesOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReturnMaterialOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OverIssueRatio = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsEmergency = table.Column<bool>(type: "bit", nullable: false),
                    TotalRequiredQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalAllocatedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalPickedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalShippedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErpCallbackStatus = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Outbound_OutboundOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Production_MaterialRequisitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequisitionStatus = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Production_MaterialRequisitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Production_SubcontractOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubcontractOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubcontractStatusValue = table.Column<int>(type: "int", nullable: false),
                    SentQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    LossRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Production_SubcontractOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_TaskCenter_WarehouseTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaskType = table.Column<int>(type: "int", nullable: false),
                    TaskPriority = table.Column<int>(type: "int", nullable: false),
                    TaskStatus = table.Column<int>(type: "int", nullable: false),
                    SourceOrderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignmentStrategy = table.Column<int>(type: "int", nullable: false),
                    ExpectedCompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualCompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SuspendedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaskProgress = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_TaskCenter_WarehouseTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Transfer_TransferOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransferOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    TransferStatus = table.Column<int>(type: "int", nullable: false),
                    SourceWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceWarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetWarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    IsCrossCompany = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Transfer_TransferOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Warehouse_Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxWeight = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    MaxCapacity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CurrentWeight = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CurrentCapacity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    StorageCondition = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BarcodeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Row = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Column = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Layer = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Warehouse_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Warehouse_Warehouse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseType = table.Column<int>(type: "int", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationUnitName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResponsibleUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponsibleUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StorageConditionType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LocationLevelCount = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Warehouse_Warehouse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Warehouse_WarehouseArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaFunction = table.Column<int>(type: "int", nullable: false),
                    StorageEnvironment = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxCapacity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CurrentCapacity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Warehouse_WarehouseArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsApprovalFlows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FlowType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsApprovalFlows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsApprovalInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstanceStatus = table.Column<int>(type: "int", nullable: false),
                    BusinessOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessOrderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BusinessOrderNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CurrentNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentNodeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubmitUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsApprovalInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsBarcodeRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BarcodeType = table.Column<int>(type: "int", nullable: false),
                    BarcodeFormat = table.Column<int>(type: "int", nullable: false),
                    CodePattern = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SeqCounter = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Prefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsBarcodeRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsBusinessRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    RuleCondition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RuleAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RuleVersion = table.Column<int>(type: "int", nullable: false),
                    EffectiveStatus = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsBusinessRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsIndustryPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PackageVersion = table.Column<int>(type: "int", nullable: false),
                    IndustryType = table.Column<int>(type: "int", nullable: false),
                    PackageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsImported = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsIndustryPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsLabelTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    TemplateContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateVersion = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IndustryStandard = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsLabelTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsNotificationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SourceEvent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SourceModule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetChannel = table.Column<int>(type: "int", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsNotificationRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SendStatus = table.Column<int>(type: "int", nullable: false),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadStatus = table.Column<int>(type: "int", nullable: false),
                    ReadTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SourceEvent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SourceModule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsNotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TemplateType = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    TemplateContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsNotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsPrintTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrinterId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PrinterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceOrderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrintContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrintQuantity = table.Column<int>(type: "int", nullable: false),
                    PrintStatus = table.Column<int>(type: "int", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxRetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsPrintTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpAuditLogActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MethodName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpAuditLogActions_AbpAuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AbpAuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeType = table.Column<byte>(type: "tinyint", nullable: false),
                    EntityTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EntityTypeFullName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityChanges_AbpAuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AbpAuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnitRoles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnitRoles", x => new { x.OrganizationUnitId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnitRoles_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoleClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserClaims_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserLogins",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderKey = table.Column<string>(type: "nvarchar(196)", maxLength: 196, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserLogins", x => new { x.UserId, x.LoginProvider });
                    table.ForeignKey(
                        name: "FK_AbpUserLogins_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserOrganizationUnits",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserOrganizationUnits", x => new { x.OrganizationUnitId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AbpUserOrganizationUnits_AbpOrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpUserOrganizationUnits_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AbpUserTokens_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wms_CycleCount_CycleCountItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DifferenceQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_CycleCount_CycleCountItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wms_CycleCount_CycleCountItems_Wms_CycleCount_CycleCountPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Wms_CycleCount_CycleCountPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Inbound_InboundLine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InboundOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PlanQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNumberList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualityStatus = table.Column<int>(type: "int", nullable: false),
                    PutawayLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PutawayLocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Inbound_InboundLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wms_Inbound_InboundLine_Wms_Inbound_InboundOrder_InboundOrderId",
                        column: x => x.InboundOrderId,
                        principalTable: "Wms_Inbound_InboundOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Inventory_InventoryAdjustmentLine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdjustmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AdjustmentQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InventoryStatusBefore = table.Column<int>(type: "int", nullable: false),
                    InventoryStatusAfter = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Inventory_InventoryAdjustmentLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wms_Inventory_InventoryAdjustmentLine_Wms_Inventory_InventoryAdjustment_AdjustmentId",
                        column: x => x.AdjustmentId,
                        principalTable: "Wms_Inventory_InventoryAdjustment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wms_LineSide_LineSideKanbanItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineSideWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    MaxQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_LineSide_LineSideKanbanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wms_LineSide_LineSideKanbanItems_Wms_LineSide_LineSideWarehouses_LineSideWarehouseId",
                        column: x => x.LineSideWarehouseId,
                        principalTable: "Wms_LineSide_LineSideWarehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Material_MaterialSubstituteRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalMaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubstituteMaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubstituteMaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubstitutePriority = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    SubstituteRatio = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 1m),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Material_MaterialSubstituteRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wms_Material_MaterialSubstituteRelation_Wms_Material_Material_OriginalMaterialId",
                        column: x => x.OriginalMaterialId,
                        principalTable: "Wms_Material_Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Outbound_OutboundLine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutboundOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequiredQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AllocatedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PickedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ShippedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PickingLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PickingLocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IssueStrategy = table.Column<int>(type: "int", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Outbound_OutboundLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wms_Outbound_OutboundLine_Wms_Outbound_OutboundOrder_OutboundOrderId",
                        column: x => x.OutboundOrderId,
                        principalTable: "Wms_Outbound_OutboundOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Production_MaterialRequisitionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequiredQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Production_MaterialRequisitionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wms_Production_MaterialRequisitionLines_Wms_Production_MaterialRequisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Wms_Production_MaterialRequisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wms_Transfer_TransferLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransferOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransferQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    OutboundConfirmedQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    InboundConfirmedQuantity = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wms_Transfer_TransferLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wms_Transfer_TransferLines_Wms_Transfer_TransferOrders_TransferOrderId",
                        column: x => x.TransferOrderId,
                        principalTable: "Wms_Transfer_TransferOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WmsApprovalNodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NodeType = table.Column<int>(type: "int", nullable: false),
                    ApproverRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApproverUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConditionExpression = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsApprovalNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WmsApprovalNodes_WmsApprovalFlows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "WmsApprovalFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WmsApprovalActionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActionTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsApprovalActionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WmsApprovalActionLogs_WmsApprovalInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "WmsApprovalInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityPropertyChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OriginalValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PropertyName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PropertyTypeFullName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityPropertyChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "AbpEntityChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogActions_AuditLogId",
                table: "AbpAuditLogActions",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogActions_TenantId_ServiceName_MethodName_ExecutionTime",
                table: "AbpAuditLogActions",
                columns: new[] { "TenantId", "ServiceName", "MethodName", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_UserId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "UserId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_AuditLogId",
                table: "AbpEntityChanges",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_TenantId_EntityTypeFullName_EntityId",
                table: "AbpEntityChanges",
                columns: new[] { "TenantId", "EntityTypeFullName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityPropertyChanges_EntityChangeId",
                table: "AbpEntityPropertyChanges",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureGroups_Name",
                table: "AbpFeatureGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_GroupName",
                table: "AbpFeatures",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_Name",
                table: "AbpFeatures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureValues_Name_ProviderName_ProviderKey",
                table: "AbpFeatureValues",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true,
                filter: "[ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_TargetTenantId",
                table: "AbpLinkUsers",
                columns: new[] { "SourceUserId", "SourceTenantId", "TargetUserId", "TargetTenantId" },
                unique: true,
                filter: "[SourceTenantId] IS NOT NULL AND [TargetTenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId",
                table: "AbpOrganizationUnitRoles",
                columns: new[] { "RoleId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_Code",
                table: "AbpOrganizationUnits",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_ParentId",
                table: "AbpOrganizationUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGrants_TenantId_Name_ProviderName_ProviderKey",
                table: "AbpPermissionGrants",
                columns: new[] { "TenantId", "Name", "ProviderName", "ProviderKey" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGroups_Name",
                table: "AbpPermissionGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_GroupName",
                table: "AbpPermissions",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_Name",
                table: "AbpPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_RoleId",
                table: "AbpRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_NormalizedName",
                table: "AbpRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_Action",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_ApplicationName",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "ApplicationName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_Identity",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "Identity" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_UserId",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_Device",
                table: "AbpSessions",
                column: "Device");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_SessionId",
                table: "AbpSessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSessions_TenantId_UserId",
                table: "AbpSessions",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettingDefinitions_Name",
                table: "AbpSettingDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_Name_ProviderName_ProviderKey",
                table: "AbpSettings",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true,
                filter: "[ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_UserId",
                table: "AbpUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_LoginProvider_ProviderKey",
                table: "AbpUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserOrganizationUnits_UserId_OrganizationUnitId",
                table: "AbpUserOrganizationUnits",
                columns: new[] { "UserId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_RoleId_UserId",
                table: "AbpUserRoles",
                columns: new[] { "RoleId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_Email",
                table: "AbpUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NormalizedEmail",
                table: "AbpUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NormalizedUserName",
                table: "AbpUsers",
                column: "NormalizedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_UserName",
                table: "AbpUsers",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_Wms_CycleCount_CycleCountItems_PlanId",
                table: "Wms_CycleCount_CycleCountItems",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IDX_CC_Status",
                table: "Wms_CycleCount_CycleCountPlans",
                column: "CountStatus");

            migrationBuilder.CreateIndex(
                name: "IDX_CC_Warehouse",
                table: "Wms_CycleCount_CycleCountPlans",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "UK_CC_PlanNo",
                table: "Wms_CycleCount_CycleCountPlans",
                column: "PlanNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_CC_ResultPlanId",
                table: "Wms_CycleCount_CycleCountResults",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IDX_IN_Line_InboundOrderId",
                table: "Wms_Inbound_InboundLine",
                column: "InboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IDX_IN_Order_CreationTime",
                table: "Wms_Inbound_InboundOrder",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IDX_IN_Order_TypeStatus",
                table: "Wms_Inbound_InboundOrder",
                columns: new[] { "InboundType", "InboundStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_IN_Order_WarehouseStatus",
                table: "Wms_Inbound_InboundOrder",
                columns: new[] { "WarehouseId", "InboundStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_IN_InboundOrderNo",
                table: "Wms_Inbound_InboundOrder",
                column: "InboundOrderNo",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Adjustment_Warehouse",
                table: "Wms_Inventory_InventoryAdjustment",
                column: "WarehouseId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_IV_AdjustmentNo",
                table: "Wms_Inventory_InventoryAdjustment",
                column: "AdjustmentNo",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Wms_Inventory_InventoryAdjustmentLine_AdjustmentId",
                table: "Wms_Inventory_InventoryAdjustmentLine",
                column: "AdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Alert_MaterialWarehouse",
                table: "Wms_Inventory_InventoryAlert",
                columns: new[] { "MaterialId", "WarehouseId", "AlertType", "IsResolved" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Balance_Batch",
                table: "Wms_Inventory_InventoryBalance",
                column: "BatchNumber",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Balance_Expiry",
                table: "Wms_Inventory_InventoryBalance",
                column: "ExpiryDate",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Balance_LastOpTime",
                table: "Wms_Inventory_InventoryBalance",
                column: "LastOperationTime");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Balance_MaterialStatus",
                table: "Wms_Inventory_InventoryBalance",
                columns: new[] { "MaterialId", "InventoryStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Balance_MaterialWarehouse",
                table: "Wms_Inventory_InventoryBalance",
                columns: new[] { "MaterialId", "WarehouseId", "InventoryStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Balance_Warehouse",
                table: "Wms_Inventory_InventoryBalance",
                columns: new[] { "WarehouseId", "InventoryStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_IV_Balance_Composite",
                table: "Wms_Inventory_InventoryBalance",
                columns: new[] { "MaterialId", "WarehouseId", "LocationId", "BatchNumber", "InventoryStatus" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_IV_FreezeOrderNo",
                table: "Wms_Inventory_InventoryFreezeOrder",
                column: "FreezeOrderNo",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Ledger_BalanceId",
                table: "Wms_Inventory_InventoryLedgerEntry",
                column: "InventoryBalanceId");

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Ledger_BalanceTime",
                table: "Wms_Inventory_InventoryLedgerEntry",
                columns: new[] { "InventoryBalanceId", "OperationTime" });

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Ledger_SourceOrder",
                table: "Wms_Inventory_InventoryLedgerEntry",
                columns: new[] { "SourceOrderType", "SourceOrderId" });

            migrationBuilder.CreateIndex(
                name: "IDX_IV_Ledger_TimeRange",
                table: "Wms_Inventory_InventoryLedgerEntry",
                column: "OperationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Wms_LineSide_LineSideKanbanItems_LineSideWarehouseId",
                table: "Wms_LineSide_LineSideKanbanItems",
                column: "LineSideWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IDX_LS_ProductionLine",
                table: "Wms_LineSide_LineSideWarehouses",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "UK_LS_Code",
                table: "Wms_LineSide_LineSideWarehouses",
                column: "LineSideWarehouseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_MT_Material_Classification",
                table: "Wms_Material_Material",
                column: "ClassificationId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_MT_Material_Name",
                table: "Wms_Material_Material",
                column: "MaterialName",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_MT_Material_Type",
                table: "Wms_Material_Material",
                column: "MaterialType",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_MT_MaterialCode",
                table: "Wms_Material_Material",
                column: "MaterialCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_MT_Classification_Parent",
                table: "Wms_Material_MaterialClassification",
                column: "ParentClassificationId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_MT_ClassificationCode",
                table: "Wms_Material_MaterialClassification",
                column: "ClassificationCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_MT_Substitute_Composite",
                table: "Wms_Material_MaterialSubstituteRelation",
                columns: new[] { "OriginalMaterialId", "SubstituteMaterialId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_MT_UnitCode",
                table: "Wms_Material_UnitOfMeasure",
                column: "UnitCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_OB_Line_OutboundOrderId",
                table: "Wms_Outbound_OutboundLine",
                column: "OutboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IDX_OB_Order_CreationTime",
                table: "Wms_Outbound_OutboundOrder",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IDX_OB_Order_IsEmergency",
                table: "Wms_Outbound_OutboundOrder",
                column: "IsEmergency",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_OB_Order_TypeStatus",
                table: "Wms_Outbound_OutboundOrder",
                columns: new[] { "OutboundType", "OutboundStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_OB_Order_WarehouseStatus",
                table: "Wms_Outbound_OutboundOrder",
                columns: new[] { "WarehouseId", "OutboundStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_OB_OutboundOrderNo",
                table: "Wms_Outbound_OutboundOrder",
                column: "OutboundOrderNo",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Wms_Production_MaterialRequisitionLines_RequisitionId",
                table: "Wms_Production_MaterialRequisitionLines",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IDX_PD_ProductionOrder",
                table: "Wms_Production_MaterialRequisitions",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "UK_PD_RequisitionNo",
                table: "Wms_Production_MaterialRequisitions",
                column: "RequisitionNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wms_TaskCenter_WarehouseTask_AssignedUserId_TaskStatus",
                table: "Wms_TaskCenter_WarehouseTask",
                columns: new[] { "AssignedUserId", "TaskStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Wms_TaskCenter_WarehouseTask_ExpectedCompletionTime_TaskStatus",
                table: "Wms_TaskCenter_WarehouseTask",
                columns: new[] { "ExpectedCompletionTime", "TaskStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Wms_TaskCenter_WarehouseTask_SourceOrderType_SourceOrderId",
                table: "Wms_TaskCenter_WarehouseTask",
                columns: new[] { "SourceOrderType", "SourceOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_Wms_TaskCenter_WarehouseTask_TaskNo",
                table: "Wms_TaskCenter_WarehouseTask",
                column: "TaskNo",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Wms_TaskCenter_WarehouseTask_TaskPriority_TaskStatus",
                table: "Wms_TaskCenter_WarehouseTask",
                columns: new[] { "TaskPriority", "TaskStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Wms_TaskCenter_WarehouseTask_WarehouseId_TaskStatus",
                table: "Wms_TaskCenter_WarehouseTask",
                columns: new[] { "WarehouseId", "TaskStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Wms_Transfer_TransferLines_TransferOrderId",
                table: "Wms_Transfer_TransferLines",
                column: "TransferOrderId");

            migrationBuilder.CreateIndex(
                name: "IDX_TF_SourceWarehouse",
                table: "Wms_Transfer_TransferOrders",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IDX_TF_Status",
                table: "Wms_Transfer_TransferOrders",
                column: "TransferStatus");

            migrationBuilder.CreateIndex(
                name: "UK_TF_TransferOrderNo",
                table: "Wms_Transfer_TransferOrders",
                column: "TransferOrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_WH_Location_BarcodeId",
                table: "Wms_Warehouse_Location",
                column: "BarcodeId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_WH_Location_WarehouseArea",
                table: "Wms_Warehouse_Location",
                columns: new[] { "WarehouseId", "AreaId" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_WH_LocationCode",
                table: "Wms_Warehouse_Location",
                column: "LocationCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_WH_OrganizationUnitId",
                table: "Wms_Warehouse_Warehouse",
                column: "OrganizationUnitId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_WH_PlantId",
                table: "Wms_Warehouse_Warehouse",
                column: "PlantId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_WH_WarehouseCode",
                table: "Wms_Warehouse_Warehouse",
                column: "WarehouseCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_WH_Area_WarehouseId",
                table: "Wms_Warehouse_WarehouseArea",
                column: "WarehouseId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UK_WH_AreaCode_Warehouse",
                table: "Wms_Warehouse_WarehouseArea",
                columns: new[] { "WarehouseId", "AreaCode" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WmsApprovalActionLogs_InstanceId",
                table: "WmsApprovalActionLogs",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IDX_WF_FlowName",
                table: "WmsApprovalFlows",
                column: "FlowName");

            migrationBuilder.CreateIndex(
                name: "IDX_WF_FlowType",
                table: "WmsApprovalFlows",
                column: "FlowType");

            migrationBuilder.CreateIndex(
                name: "IDX_WF_IsActive",
                table: "WmsApprovalFlows",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IDX_WF_BusinessOrder",
                table: "WmsApprovalInstances",
                columns: new[] { "BusinessOrderType", "BusinessOrderId" });

            migrationBuilder.CreateIndex(
                name: "IDX_WF_CurrentNodeId",
                table: "WmsApprovalInstances",
                column: "CurrentNodeId");

            migrationBuilder.CreateIndex(
                name: "IDX_WF_InstanceStatus",
                table: "WmsApprovalInstances",
                column: "InstanceStatus");

            migrationBuilder.CreateIndex(
                name: "IDX_WF_SubmitUserId",
                table: "WmsApprovalInstances",
                column: "SubmitUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WmsApprovalNodes_FlowId",
                table: "WmsApprovalNodes",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_BarcodeRule_IsActive",
                table: "WmsBarcodeRules",
                column: "IsActive",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_BarcodeRule_Name",
                table: "WmsBarcodeRules",
                column: "RuleName",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_BarcodeRule_Type",
                table: "WmsBarcodeRules",
                column: "BarcodeType",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Rule_CreationTime",
                table: "WmsBusinessRules",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Rule_EffectiveStatus",
                table: "WmsBusinessRules",
                column: "EffectiveStatus",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Rule_RuleName",
                table: "WmsBusinessRules",
                column: "RuleName",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Rule_RuleType",
                table: "WmsBusinessRules",
                column: "RuleType",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Rule_TypeStatus",
                table: "WmsBusinessRules",
                columns: new[] { "RuleType", "EffectiveStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Rule_Version",
                table: "WmsBusinessRules",
                column: "RuleVersion",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Package_CreationTime",
                table: "WmsIndustryPackages",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Package_IndustryType",
                table: "WmsIndustryPackages",
                column: "IndustryType",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_RE_Package_PackageName",
                table: "WmsIndustryPackages",
                column: "PackageName",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_LabelTemplate_IsActive",
                table: "WmsLabelTemplates",
                column: "IsActive",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_LabelTemplate_Name",
                table: "WmsLabelTemplates",
                column: "TemplateName",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_LabelTemplate_Type",
                table: "WmsLabelTemplates",
                column: "TemplateType",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NTR_IsEnabled",
                table: "WmsNotificationRules",
                column: "IsEnabled",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NTR_SourceEventModule",
                table: "WmsNotificationRules",
                columns: new[] { "SourceEvent", "SourceModule" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NTR_TargetChannel",
                table: "WmsNotificationRules",
                column: "TargetChannel",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NT_CorrelationId",
                table: "WmsNotifications",
                column: "CorrelationId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NT_NotificationType",
                table: "WmsNotifications",
                column: "NotificationType",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NT_RecipientReadStatus",
                table: "WmsNotifications",
                columns: new[] { "RecipientId", "ReadStatus" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NT_SendStatus",
                table: "WmsNotifications",
                column: "SendStatus",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NT_SendTime",
                table: "WmsNotifications",
                column: "SendTime",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NTT_Channel",
                table: "WmsNotificationTemplates",
                column: "Channel",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NTT_IsActive",
                table: "WmsNotificationTemplates",
                column: "IsActive",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_NTT_TemplateName",
                table: "WmsNotificationTemplates",
                column: "TemplateName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_PrintTask_Printer",
                table: "WmsPrintTasks",
                column: "PrinterId",
                filter: "[IsDeleted] = 0 AND [PrinterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_PrintTask_SourceOrder",
                table: "WmsPrintTasks",
                columns: new[] { "SourceOrderType", "SourceOrderId" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_PrintTask_Status",
                table: "WmsPrintTasks",
                column: "PrintStatus",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IDX_BL_PrintTask_TaskNo",
                table: "WmsPrintTasks",
                column: "TaskNo",
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpAuditLogActions");

            migrationBuilder.DropTable(
                name: "AbpBackgroundJobs");

            migrationBuilder.DropTable(
                name: "AbpClaimTypes");

            migrationBuilder.DropTable(
                name: "AbpEntityPropertyChanges");

            migrationBuilder.DropTable(
                name: "AbpFeatureGroups");

            migrationBuilder.DropTable(
                name: "AbpFeatures");

            migrationBuilder.DropTable(
                name: "AbpFeatureValues");

            migrationBuilder.DropTable(
                name: "AbpLinkUsers");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnitRoles");

            migrationBuilder.DropTable(
                name: "AbpPermissionGrants");

            migrationBuilder.DropTable(
                name: "AbpPermissionGroups");

            migrationBuilder.DropTable(
                name: "AbpPermissions");

            migrationBuilder.DropTable(
                name: "AbpRoleClaims");

            migrationBuilder.DropTable(
                name: "AbpSecurityLogs");

            migrationBuilder.DropTable(
                name: "AbpSessions");

            migrationBuilder.DropTable(
                name: "AbpSettingDefinitions");

            migrationBuilder.DropTable(
                name: "AbpSettings");

            migrationBuilder.DropTable(
                name: "AbpUserClaims");

            migrationBuilder.DropTable(
                name: "AbpUserDelegations");

            migrationBuilder.DropTable(
                name: "AbpUserLogins");

            migrationBuilder.DropTable(
                name: "AbpUserOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpUserRoles");

            migrationBuilder.DropTable(
                name: "AbpUserTokens");

            migrationBuilder.DropTable(
                name: "Wms_CycleCount_CycleCountItems");

            migrationBuilder.DropTable(
                name: "Wms_CycleCount_CycleCountResults");

            migrationBuilder.DropTable(
                name: "Wms_Inbound_InboundLine");

            migrationBuilder.DropTable(
                name: "Wms_Inventory_InventoryAdjustmentLine");

            migrationBuilder.DropTable(
                name: "Wms_Inventory_InventoryAlert");

            migrationBuilder.DropTable(
                name: "Wms_Inventory_InventoryBalance");

            migrationBuilder.DropTable(
                name: "Wms_Inventory_InventoryFreezeOrder");

            migrationBuilder.DropTable(
                name: "Wms_Inventory_InventoryLedgerEntry");

            migrationBuilder.DropTable(
                name: "Wms_LineSide_LineSideKanbanItems");

            migrationBuilder.DropTable(
                name: "Wms_Material_MaterialClassification");

            migrationBuilder.DropTable(
                name: "Wms_Material_MaterialSubstituteRelation");

            migrationBuilder.DropTable(
                name: "Wms_Material_UnitOfMeasure");

            migrationBuilder.DropTable(
                name: "Wms_Outbound_OutboundLine");

            migrationBuilder.DropTable(
                name: "Wms_Production_MaterialRequisitionLines");

            migrationBuilder.DropTable(
                name: "Wms_Production_SubcontractOrders");

            migrationBuilder.DropTable(
                name: "Wms_TaskCenter_WarehouseTask");

            migrationBuilder.DropTable(
                name: "Wms_Transfer_TransferLines");

            migrationBuilder.DropTable(
                name: "Wms_Warehouse_Location");

            migrationBuilder.DropTable(
                name: "Wms_Warehouse_Warehouse");

            migrationBuilder.DropTable(
                name: "Wms_Warehouse_WarehouseArea");

            migrationBuilder.DropTable(
                name: "WmsApprovalActionLogs");

            migrationBuilder.DropTable(
                name: "WmsApprovalNodes");

            migrationBuilder.DropTable(
                name: "WmsBarcodeRules");

            migrationBuilder.DropTable(
                name: "WmsBusinessRules");

            migrationBuilder.DropTable(
                name: "WmsIndustryPackages");

            migrationBuilder.DropTable(
                name: "WmsLabelTemplates");

            migrationBuilder.DropTable(
                name: "WmsNotificationRules");

            migrationBuilder.DropTable(
                name: "WmsNotifications");

            migrationBuilder.DropTable(
                name: "WmsNotificationTemplates");

            migrationBuilder.DropTable(
                name: "WmsPrintTasks");

            migrationBuilder.DropTable(
                name: "AbpEntityChanges");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpRoles");

            migrationBuilder.DropTable(
                name: "AbpUsers");

            migrationBuilder.DropTable(
                name: "Wms_CycleCount_CycleCountPlans");

            migrationBuilder.DropTable(
                name: "Wms_Inbound_InboundOrder");

            migrationBuilder.DropTable(
                name: "Wms_Inventory_InventoryAdjustment");

            migrationBuilder.DropTable(
                name: "Wms_LineSide_LineSideWarehouses");

            migrationBuilder.DropTable(
                name: "Wms_Material_Material");

            migrationBuilder.DropTable(
                name: "Wms_Outbound_OutboundOrder");

            migrationBuilder.DropTable(
                name: "Wms_Production_MaterialRequisitions");

            migrationBuilder.DropTable(
                name: "Wms_Transfer_TransferOrders");

            migrationBuilder.DropTable(
                name: "WmsApprovalInstances");

            migrationBuilder.DropTable(
                name: "WmsApprovalFlows");

            migrationBuilder.DropTable(
                name: "AbpAuditLogs");
        }
    }
}
