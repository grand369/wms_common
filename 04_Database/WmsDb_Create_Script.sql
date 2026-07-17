-- ============================================================================
-- Manufacturing WMS Platform — SQL Server Database Creation Script
-- ============================================================================
-- 基于: Phase5_Database_Design.md v1.0
-- 目标: SQL Server 2019+
-- 数据库: WmsDb
-- 表数量: 42+ (含子实体表)
-- 生成日期: 2026-06-30
-- ============================================================================

-- ============================================================================
-- PART 0: 数据库创建
-- ============================================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'WmsDb')
BEGIN
    CREATE DATABASE WmsDb
    COLLATE Chinese_PRC_CI_AS;
END
GO

USE WmsDb;
GO

-- ============================================================================
-- PART 1: ABP 身份认证表 (用户/角色/权限)
-- ============================================================================

-- 1.1 角色表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpRoles')
BEGIN
CREATE TABLE AbpRoles (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    Name            NVARCHAR(256)       NOT NULL,
    NormalizedName  NVARCHAR(256)       NOT NULL,
    ConcurrencyStamp NVARCHAR(256)      NULL,
    IsDefault       BIT                 NOT NULL DEFAULT 0,
    IsPublic        BIT                 NOT NULL DEFAULT 0,
    IsStatic        BIT                 NOT NULL DEFAULT 0,
    ExtraProperties NVARCHAR(MAX)       NULL,
    -- ABP Audit
    CreationTime        DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER NULL,
    LastModificationTime DATETIME2      NULL,
    LastModifierId      UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT             NOT NULL DEFAULT 0,
    DeleterId           UNIQUEIDENTIFIER NULL,
    DeletionTime        DATETIME2       NULL,
    CONSTRAINT PK_AbpRoles PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_AbpRoles_Name UNIQUE (NormalizedName)
);
END
GO

-- 1.2 用户表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpUsers')
BEGIN
CREATE TABLE AbpUsers (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    UserName            NVARCHAR(256)       NOT NULL,
    NormalizedUserName  NVARCHAR(256)       NOT NULL,
    Name                NVARCHAR(64)        NULL,
    Surname             NVARCHAR(64)        NULL,
    Email               NVARCHAR(256)       NULL,
    NormalizedEmail     NVARCHAR(256)       NULL,
    EmailConfirmed      BIT                 NOT NULL DEFAULT 0,
    PasswordHash        NVARCHAR(256)       NULL,
    SecurityStamp       NVARCHAR(256)       NULL,
    ConcurrencyStamp    NVARCHAR(256)       NULL,
    PhoneNumber         NVARCHAR(16)        NULL,
    PhoneNumberConfirmed BIT                NOT NULL DEFAULT 0,
    TwoFactorEnabled    BIT                 NOT NULL DEFAULT 0,
    LockoutEnd          DATETIMEOFFSET      NULL,
    LockoutEnabled      BIT                 NOT NULL DEFAULT 1,
    AccessFailedCount   INT                 NOT NULL DEFAULT 0,
    IsActive            BIT                 NOT NULL DEFAULT 1,
    IsExternal          BIT                 NOT NULL DEFAULT 0,
    TenantId            UNIQUEIDENTIFIER    NULL,
    ExtraProperties     NVARCHAR(MAX)       NULL,
    -- ABP Audit
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER    NULL,
    LastModificationTime DATETIME2          NULL,
    LastModifierId      UNIQUEIDENTIFIER    NULL,
    IsDeleted           BIT                 NOT NULL DEFAULT 0,
    DeleterId           UNIQUEIDENTIFIER    NULL,
    DeletionTime        DATETIME2           NULL,
    CONSTRAINT PK_AbpUsers PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_AbpUsers_UserName UNIQUE (NormalizedUserName)
);
END
GO

-- 1.3 用户角色关联表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpUserRoles')
BEGIN
CREATE TABLE AbpUserRoles (
    UserId  UNIQUEIDENTIFIER NOT NULL,
    RoleId  UNIQUEIDENTIFIER NOT NULL,
    TenantId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_AbpUserRoles PRIMARY KEY CLUSTERED (UserId, RoleId)
);
END
GO

-- 1.4 组织单元表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpOrganizationUnits')
BEGIN
CREATE TABLE AbpOrganizationUnits (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    ParentId    UNIQUEIDENTIFIER    NULL,
    Code        NVARCHAR(95)        NOT NULL,
    DisplayName NVARCHAR(128)       NOT NULL,
    TenantId    UNIQUEIDENTIFIER    NULL,
    ExtraProperties NVARCHAR(MAX)   NULL,
    CreationTime        DATETIME2   NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER NULL,
    LastModificationTime DATETIME2  NULL,
    LastModifierId      UNIQUEIDENTIFIER NULL,
    IsDeleted           BIT         NOT NULL DEFAULT 0,
    DeleterId           UNIQUEIDENTIFIER NULL,
    DeletionTime        DATETIME2   NULL,
    CONSTRAINT PK_AbpOrganizationUnits PRIMARY KEY CLUSTERED (Id)
);
END
GO


-- ============================================================================
-- PART 2: BC-01 Warehouse Context (仓库主数据)
-- ============================================================================

-- 2.1 仓库表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Warehouse_Warehouse')
BEGIN
CREATE TABLE Wms_Warehouse_Warehouse (
    Id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    WarehouseCode           NVARCHAR(50)        NOT NULL,
    WarehouseName           NVARCHAR(200)       NOT NULL,
    WarehouseType           INT                 NOT NULL,
    OrganizationUnitId      UNIQUEIDENTIFIER    NOT NULL,
    OrganizationUnitName    NVARCHAR(200)       NOT NULL,
    PlantId                 UNIQUEIDENTIFIER    NOT NULL,
    PlantName               NVARCHAR(100)       NOT NULL,
    ResponsibleUserId       UNIQUEIDENTIFIER    NULL,
    ResponsibleUserName     NVARCHAR(100)       NULL,
    Address                 NVARCHAR(500)       NULL,
    StorageConditionType    INT                 NULL DEFAULT 0,
    LocationLevelCount      INT                 NOT NULL DEFAULT 3,
    IsActive                BIT                 NOT NULL DEFAULT 1,
    Remark                  NVARCHAR(1000)      NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER    NULL,
    LastModificationTime    DATETIME2           NULL,
    LastModifierId          UNIQUEIDENTIFIER    NULL,
    IsDeleted               BIT                 NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER    NULL,
    DeletionTime            DATETIME2           NULL,
    CONSTRAINT PK_Wms_Warehouse_Warehouse PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Warehouse_WarehouseCode UNIQUE (WarehouseCode)
);
END
GO

-- 2.2 库区表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Warehouse_WarehouseArea')
BEGIN
CREATE TABLE Wms_Warehouse_WarehouseArea (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    AreaCode            NVARCHAR(50)        NOT NULL,
    AreaName            NVARCHAR(200)       NOT NULL,
    WarehouseId         UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode       NVARCHAR(50)        NOT NULL,
    AreaFunction        INT                 NOT NULL,
    StorageEnvironment  INT                 NULL DEFAULT 0,
    MaxCapacity         DECIMAL(18,4)       NULL,
    CurrentCapacity     DECIMAL(18,4)       NULL,
    IsActive            BIT                 NOT NULL DEFAULT 1,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Warehouse_WarehouseArea PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Warehouse_AreaCode_Warehouse UNIQUE (WarehouseId, AreaCode)
);
END
GO

-- 2.3 库位表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Warehouse_Location')
BEGIN
CREATE TABLE Wms_Warehouse_Location (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    LocationCode    NVARCHAR(50)        NOT NULL,
    WarehouseId     UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode   NVARCHAR(50)        NOT NULL,
    AreaId          UNIQUEIDENTIFIER    NOT NULL,
    AreaCode        NVARCHAR(50)        NOT NULL,
    LocationType    INT                 NULL DEFAULT 0,
    MaxWeight       DECIMAL(18,4)       NULL,
    MaxCapacity     DECIMAL(18,4)       NULL,
    CurrentWeight   DECIMAL(18,4)       NULL,
    CurrentCapacity DECIMAL(18,4)       NULL,
    StorageCondition INT               NULL DEFAULT 0,
    BarcodeId       NVARCHAR(100)       NOT NULL,
    [Row]           NVARCHAR(10)        NULL,
    [Column]        NVARCHAR(10)        NULL,
    [Layer]         NVARCHAR(10)        NULL,
    IsActive        BIT                 NOT NULL DEFAULT 1,
    -- ABP Audit Fields
    CreationTime            DATETIME2   NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2   NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT         NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2   NULL,
    CONSTRAINT PK_Wms_Warehouse_Location PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Warehouse_LocationCode UNIQUE (LocationCode)
);
END
GO

-- Warehouse 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Warehouse_Area_WarehouseId')
    CREATE NONCLUSTERED INDEX IDX_Wms_Warehouse_Area_WarehouseId ON Wms_Warehouse_WarehouseArea (WarehouseId) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Warehouse_Location_WarehouseId')
    CREATE NONCLUSTERED INDEX IDX_Wms_Warehouse_Location_WarehouseId ON Wms_Warehouse_Location (WarehouseId) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Warehouse_Location_AreaId')
    CREATE NONCLUSTERED INDEX IDX_Wms_Warehouse_Location_AreaId ON Wms_Warehouse_Location (AreaId) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 3: BC-02 Material Context (物料主数据)
-- ============================================================================

-- 3.1 物料分类表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Material_MaterialClassification')
BEGIN
CREATE TABLE Wms_Material_MaterialClassification (
    Id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    ClassificationCode      NVARCHAR(50)        NOT NULL,
    ClassificationName      NVARCHAR(200)       NOT NULL,
    ParentClassificationId  UNIQUEIDENTIFIER    NULL,
    ClassificationLevel     INT                 NOT NULL DEFAULT 1,
    AttributeTemplateId     UNIQUEIDENTIFIER    NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER    NULL,
    LastModificationTime    DATETIME2           NULL,
    LastModifierId          UNIQUEIDENTIFIER    NULL,
    IsDeleted               BIT                 NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER    NULL,
    DeletionTime            DATETIME2           NULL,
    CONSTRAINT PK_Wms_Material_MaterialClassification PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Material_ClassificationCode UNIQUE (ClassificationCode)
);
END
GO

-- 3.2 计量单位表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Material_UnitOfMeasure')
BEGIN
CREATE TABLE Wms_Material_UnitOfMeasure (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    UnitCode    NVARCHAR(50)        NOT NULL,
    UnitName    NVARCHAR(100)       NOT NULL,
    UnitSymbol  NVARCHAR(20)        NOT NULL,
    UnitType    INT                 NOT NULL,
    IsActive    BIT                 NOT NULL DEFAULT 1,
    -- ABP Audit Fields
    CreationTime            DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2 NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT       NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2 NULL,
    CONSTRAINT PK_Wms_Material_UnitOfMeasure PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Material_UnitCode UNIQUE (UnitCode)
);
END
GO

-- 3.3 物料表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Material_Material')
BEGIN
CREATE TABLE Wms_Material_Material (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    MaterialCode        NVARCHAR(50)        NOT NULL,
    MaterialName        NVARCHAR(200)       NOT NULL,
    MaterialNameEn      NVARCHAR(200)       NULL,
    ClassificationId    UNIQUEIDENTIFIER    NULL,
    Specification       NVARCHAR(500)       NULL,
    PrimaryUnitId       UNIQUEIDENTIFIER    NOT NULL,
    PrimaryUnitName     NVARCHAR(50)        NOT NULL,
    SecondaryUnitId     UNIQUEIDENTIFIER    NULL,
    ConversionRate      DECIMAL(18,6)       NULL,
    MaterialType        INT                 NOT NULL,
    StorageAttribute    NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    QualityAttribute    NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    InventoryAttribute  NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    IssueStrategy       NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    DangerAttribute     NVARCHAR(MAX)       NULL,
    IsActive            BIT                 NOT NULL DEFAULT 1,
    ErpSyncStatus       INT                 NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Material_Material PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Material_MaterialCode UNIQUE (MaterialCode)
);
END
GO

-- 3.4 替代料关系表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Material_MaterialSubstituteRelation')
BEGIN
CREATE TABLE Wms_Material_MaterialSubstituteRelation (
    Id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    OriginalMaterialId      UNIQUEIDENTIFIER    NOT NULL,
    SubstituteMaterialId    UNIQUEIDENTIFIER    NOT NULL,
    SubstituteMaterialCode  NVARCHAR(50)        NOT NULL,
    SubstitutePriority      INT                 NOT NULL DEFAULT 1,
    SubstituteRatio         DECIMAL(18,6)       NOT NULL DEFAULT 1.0,
    -- ABP Audit Fields
    CreationTime            DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER    NULL,
    LastModificationTime    DATETIME2           NULL,
    LastModifierId          UNIQUEIDENTIFIER    NULL,
    IsDeleted               BIT                 NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER    NULL,
    DeletionTime            DATETIME2           NULL,
    CONSTRAINT PK_Wms_Material_SubstituteRelation PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Material_Substitute_Composite UNIQUE (OriginalMaterialId, SubstituteMaterialId)
);
END
GO

-- Material 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Material_Classification')
    CREATE NONCLUSTERED INDEX IDX_Wms_Material_Classification ON Wms_Material_Material (ClassificationId) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Material_MaterialType')
    CREATE NONCLUSTERED INDEX IDX_Wms_Material_MaterialType ON Wms_Material_Material (MaterialType) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Material_Name')
    CREATE NONCLUSTERED INDEX IDX_Wms_Material_Name ON Wms_Material_Material (MaterialName) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 4: BC-03 Inventory Context (库存核心) ⚠️
-- ============================================================================

-- 4.1 库存余额表 ⚠️核心表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Inventory_InventoryBalance')
BEGIN
CREATE TABLE Wms_Inventory_InventoryBalance (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    MaterialId          UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode        NVARCHAR(50)        NOT NULL,
    WarehouseId         UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode       NVARCHAR(50)        NOT NULL,
    LocationId          UNIQUEIDENTIFIER    NOT NULL,
    LocationCode        NVARCHAR(50)        NOT NULL,
    BatchNumber         NVARCHAR(50)        NULL,
    InventoryStatus     INT                 NOT NULL DEFAULT 0,
    Quantity            DECIMAL(18,4)       NOT NULL DEFAULT 0,
    ReservedQuantity    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    FrozenQuantity      DECIMAL(18,4)       NOT NULL DEFAULT 0,
    InTransitQuantity   DECIMAL(18,4)       NOT NULL DEFAULT 0,
    AvailableQuantity   DECIMAL(18,4)       NOT NULL DEFAULT 0,
    ExpiryDate          DATETIME2           NULL,
    ProductionDate      DATETIME2           NULL,
    SupplierId          UNIQUEIDENTIFIER    NULL,
    SupplierName        NVARCHAR(100)       NULL,
    UnitCost            DECIMAL(18,6)       NULL,
    LastOperationTime   DATETIME2           NOT NULL DEFAULT GETDATE(),
    ConcurrencyVersion  INT                 NOT NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Inventory_InventoryBalance PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Inventory_Balance_Composite UNIQUE (MaterialId, WarehouseId, LocationId, BatchNumber, InventoryStatus)
);
END
GO

-- 4.2 库存台账表 ⚠️不可修改不可删除
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Inventory_InventoryLedgerEntry')
BEGIN
CREATE TABLE Wms_Inventory_InventoryLedgerEntry (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    InventoryBalanceId  UNIQUEIDENTIFIER    NOT NULL,
    OperationType       INT                 NOT NULL,
    OperationQuantity   DECIMAL(18,4)       NOT NULL,
    BeforeQuantity      DECIMAL(18,4)       NOT NULL,
    AfterQuantity       DECIMAL(18,4)       NOT NULL,
    BeforeAvailable     DECIMAL(18,4)       NOT NULL,
    AfterAvailable      DECIMAL(18,4)       NOT NULL,
    OperationTime       DATETIME2           NOT NULL DEFAULT GETDATE(),
    OperatorId          UNIQUEIDENTIFIER    NOT NULL,
    OperatorName        NVARCHAR(100)       NOT NULL,
    SourceOrderType     NVARCHAR(50)        NOT NULL,
    SourceOrderId       UNIQUEIDENTIFIER    NOT NULL,
    SourceOrderNo       NVARCHAR(50)        NOT NULL,
    Remark              NVARCHAR(500)       NULL,
    -- ⚠️仅创建审计，无修改/删除字段
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER    NOT NULL,
    CONSTRAINT PK_Wms_Inventory_InventoryLedgerEntry PRIMARY KEY CLUSTERED (Id)
);
-- ⚠️禁止修改和删除
DENY UPDATE, DELETE ON Wms_Inventory_InventoryLedgerEntry TO PUBLIC;
END
GO

-- 4.3 库存调整单表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Inventory_InventoryAdjustment')
BEGIN
CREATE TABLE Wms_Inventory_InventoryAdjustment (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    AdjustmentNo        NVARCHAR(50)        NOT NULL,
    AdjustmentType      INT                 NOT NULL,
    AdjustmentReason    NVARCHAR(500)       NOT NULL,
    ApprovalStatus      INT                 NOT NULL DEFAULT 0,
    WarehouseId         UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode       NVARCHAR(50)        NOT NULL,
    IsCompleted         BIT                 NOT NULL DEFAULT 0,
    CompletionTime      DATETIME2           NULL,
    Remark              NVARCHAR(1000)      NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Inventory_InventoryAdjustment PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Inventory_AdjustmentNo UNIQUE (AdjustmentNo)
);
END
GO

-- 4.4 库存调整行表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Inventory_InventoryAdjustmentLine')
BEGIN
CREATE TABLE Wms_Inventory_InventoryAdjustmentLine (
    Id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    AdjustmentId            UNIQUEIDENTIFIER    NOT NULL,
    LineNo                  INT                 NOT NULL,
    MaterialId              UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode            NVARCHAR(50)        NOT NULL,
    MaterialName            NVARCHAR(200)       NOT NULL,
    AdjustmentQuantity      DECIMAL(18,4)       NOT NULL,
    LocationId              UNIQUEIDENTIFIER    NOT NULL,
    LocationCode            NVARCHAR(50)        NOT NULL,
    BatchNumber             NVARCHAR(50)        NULL,
    InventoryStatusBefore   INT                 NOT NULL,
    InventoryStatusAfter    INT                 NOT NULL,
    Reason                  NVARCHAR(500)       NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER    NULL,
    LastModificationTime    DATETIME2           NULL,
    LastModifierId          UNIQUEIDENTIFIER    NULL,
    IsDeleted               BIT                 NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER    NULL,
    DeletionTime            DATETIME2           NULL,
    CONSTRAINT PK_Wms_Inventory_AdjustmentLine PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Inventory_AdjLine_Composite UNIQUE (AdjustmentId, LineNo)
);
END
GO

-- 4.5 库存冻结单表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Inventory_InventoryFreezeOrder')
BEGIN
CREATE TABLE Wms_Inventory_InventoryFreezeOrder (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    FreezeOrderNo   NVARCHAR(50)        NOT NULL,
    FreezeScope     INT                 NOT NULL,
    FreezeReason    NVARCHAR(500)       NOT NULL,
    FreezeStatus    INT                 NOT NULL DEFAULT 0,
    WarehouseId     UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode   NVARCHAR(50)        NOT NULL,
    IsApproved      BIT                 NOT NULL DEFAULT 0,
    FreezeStartTime DATETIME2           NOT NULL,
    FreezeEndTime   DATETIME2           NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2   NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2   NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT         NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2   NULL,
    CONSTRAINT PK_Wms_Inventory_FreezeOrder PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Inventory_FreezeOrderNo UNIQUE (FreezeOrderNo)
);
END
GO

-- 4.6 库存预警表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Inventory_InventoryAlert')
BEGIN
CREATE TABLE Wms_Inventory_InventoryAlert (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    AlertType           INT                 NOT NULL,
    MaterialId          UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode        NVARCHAR(50)        NOT NULL,
    WarehouseId         UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode       NVARCHAR(50)        NOT NULL,
    CurrentQuantity     DECIMAL(18,4)       NOT NULL,
    ThresholdQuantity   DECIMAL(18,4)       NOT NULL,
    IsResolved          BIT                 NOT NULL DEFAULT 0,
    AlertTime           DATETIME2           NOT NULL DEFAULT GETDATE(),
    ResolveTime         DATETIME2           NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Inventory_Alert PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- ⚠️ Inventory 核心索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Balance_MaterialWarehouse')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Balance_MaterialWarehouse
    ON Wms_Inventory_InventoryBalance (MaterialId, WarehouseId, InventoryStatus)
    INCLUDE (AvailableQuantity, Quantity, ReservedQuantity, FrozenQuantity)
    WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Balance_Warehouse')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Balance_Warehouse
    ON Wms_Inventory_InventoryBalance (WarehouseId, InventoryStatus)
    INCLUDE (MaterialId, MaterialCode, AvailableQuantity)
    WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Balance_MaterialStatus')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Balance_MaterialStatus
    ON Wms_Inventory_InventoryBalance (MaterialId, InventoryStatus)
    INCLUDE (WarehouseId, LocationCode, AvailableQuantity)
    WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Balance_Expiry')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Balance_Expiry
    ON Wms_Inventory_InventoryBalance (ExpiryDate)
    INCLUDE (MaterialId, MaterialCode, WarehouseId, BatchNumber)
    WHERE IsDeleted = 0 AND ExpiryDate IS NOT NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Balance_Batch')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Balance_Batch
    ON Wms_Inventory_InventoryBalance (BatchNumber)
    INCLUDE (MaterialId, WarehouseId, InventoryStatus, Quantity)
    WHERE IsDeleted = 0 AND BatchNumber IS NOT NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Balance_LastOpTime')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Balance_LastOpTime
    ON Wms_Inventory_InventoryBalance (LastOperationTime DESC)
    WHERE IsDeleted = 0;
GO

-- InventoryLedger 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Ledger_BalanceId')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Ledger_BalanceId ON Wms_Inventory_InventoryLedgerEntry (InventoryBalanceId);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Ledger_SourceOrder')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Ledger_SourceOrder ON Wms_Inventory_InventoryLedgerEntry (SourceOrderType, SourceOrderId);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inventory_Ledger_OperationTime')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inventory_Ledger_OperationTime ON Wms_Inventory_InventoryLedgerEntry (OperationTime DESC);
GO


-- ============================================================================
-- PART 5: BC-04 Inbound Context (入库)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Inbound_InboundOrder')
BEGIN
CREATE TABLE Wms_Inbound_InboundOrder (
    Id                          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    InboundOrderNo              NVARCHAR(50)        NOT NULL,
    InboundType                 INT                 NOT NULL,
    InboundStatus               INT                 NOT NULL DEFAULT 0,
    WarehouseId                 UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode               NVARCHAR(50)        NOT NULL,
    PurchaseOrderId             UNIQUEIDENTIFIER    NULL,
    PurchaseOrderNo             NVARCHAR(50)        NULL,
    ProductionOrderId           UNIQUEIDENTIFIER    NULL,
    ReturnOrderId               UNIQUEIDENTIFIER    NULL,
    SupplierId                  UNIQUEIDENTIFIER    NULL,
    SupplierName                NVARCHAR(100)       NULL,
    OverReceiptRatio            DECIMAL(18,4)       NOT NULL DEFAULT 0,
    QualityInspectionRequired   BIT                 NOT NULL DEFAULT 1,
    TotalPlanQuantity           DECIMAL(18,4)       NOT NULL DEFAULT 0,
    TotalReceivedQuantity       DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IsCompleted                 BIT                 NOT NULL DEFAULT 0,
    CompletionTime              DATETIME2           NULL,
    ErpCallbackStatus           INT                 NULL DEFAULT 0,
    Remark                      NVARCHAR(1000)      NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2               NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER        NULL,
    LastModificationTime    DATETIME2               NULL,
    LastModifierId          UNIQUEIDENTIFIER        NULL,
    IsDeleted               BIT                     NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER        NULL,
    DeletionTime            DATETIME2               NULL,
    CONSTRAINT PK_Wms_Inbound_InboundOrder PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Inbound_InboundOrderNo UNIQUE (InboundOrderNo)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Inbound_InboundLine')
BEGIN
CREATE TABLE Wms_Inbound_InboundLine (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    InboundOrderId      UNIQUEIDENTIFIER    NOT NULL,
    LineNo              INT                 NOT NULL,
    MaterialId          UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode        NVARCHAR(50)        NOT NULL,
    MaterialName        NVARCHAR(200)       NOT NULL,
    PlanQuantity        DECIMAL(18,4)       NOT NULL DEFAULT 0,
    ReceivedQuantity    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    BatchNumber         NVARCHAR(50)        NULL,
    SerialNumberList    NVARCHAR(MAX)       NULL,
    QualityStatus       INT                 NOT NULL DEFAULT 0,
    PutawayLocationId   UNIQUEIDENTIFIER    NULL,
    PutawayLocationCode NVARCHAR(50)        NULL,
    ExpiryDate          DATETIME2           NULL,
    ProductionDate      DATETIME2           NULL,
    Remark              NVARCHAR(500)       NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Inbound_InboundLine PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Inbound_Line_Composite UNIQUE (InboundOrderId, LineNo)
);
END
GO

-- Inbound 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inbound_WarehouseStatus')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inbound_WarehouseStatus ON Wms_Inbound_InboundOrder (WarehouseId, InboundStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inbound_TypeStatus')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inbound_TypeStatus ON Wms_Inbound_InboundOrder (InboundType, InboundStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Inbound_CreationTime')
    CREATE NONCLUSTERED INDEX IDX_Wms_Inbound_CreationTime ON Wms_Inbound_InboundOrder (CreationTime DESC) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 6: BC-05 Outbound Context (出库)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Outbound_OutboundOrder')
BEGIN
CREATE TABLE Wms_Outbound_OutboundOrder (
    Id                          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    OutboundOrderNo             NVARCHAR(50)        NOT NULL,
    OutboundType                INT                 NOT NULL,
    OutboundStatus              INT                 NOT NULL DEFAULT 0,
    WarehouseId                 UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode               NVARCHAR(50)        NOT NULL,
    MaterialRequisitionId       UNIQUEIDENTIFIER    NULL,
    SalesOrderId                UNIQUEIDENTIFIER    NULL,
    ReturnMaterialOrderId       UNIQUEIDENTIFIER    NULL,
    OverIssueRatio              DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IsEmergency                 BIT                 NOT NULL DEFAULT 0,
    TotalRequiredQuantity       DECIMAL(18,4)       NOT NULL DEFAULT 0,
    TotalAllocatedQuantity      DECIMAL(18,4)       NOT NULL DEFAULT 0,
    TotalPickedQuantity         DECIMAL(18,4)       NOT NULL DEFAULT 0,
    TotalShippedQuantity        DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IsCompleted                 BIT                 NOT NULL DEFAULT 0,
    CompletionTime              DATETIME2           NULL,
    ErpCallbackStatus           INT                 NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2               NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER        NULL,
    LastModificationTime    DATETIME2               NULL,
    LastModifierId          UNIQUEIDENTIFIER        NULL,
    IsDeleted               BIT                     NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER        NULL,
    DeletionTime            DATETIME2               NULL,
    CONSTRAINT PK_Wms_Outbound_OutboundOrder PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Outbound_OutboundOrderNo UNIQUE (OutboundOrderNo)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Outbound_OutboundLine')
BEGIN
CREATE TABLE Wms_Outbound_OutboundLine (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    OutboundOrderId     UNIQUEIDENTIFIER    NOT NULL,
    LineNo              INT                 NOT NULL,
    MaterialId          UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode        NVARCHAR(50)        NOT NULL,
    MaterialName        NVARCHAR(200)       NOT NULL,
    RequiredQuantity    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    AllocatedQuantity   DECIMAL(18,4)       NOT NULL DEFAULT 0,
    PickedQuantity      DECIMAL(18,4)       NOT NULL DEFAULT 0,
    ShippedQuantity     DECIMAL(18,4)       NOT NULL DEFAULT 0,
    PickingLocationId   UNIQUEIDENTIFIER    NULL,
    PickingLocationCode NVARCHAR(50)        NULL,
    IssueStrategyType   INT                 NOT NULL DEFAULT 0,
    BatchNumber         NVARCHAR(50)        NULL,
    Remark              NVARCHAR(500)       NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Outbound_OutboundLine PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Outbound_Line_Composite UNIQUE (OutboundOrderId, LineNo)
);
END
GO

-- Outbound 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Outbound_WarehouseStatus')
    CREATE NONCLUSTERED INDEX IDX_Wms_Outbound_WarehouseStatus ON Wms_Outbound_OutboundOrder (WarehouseId, OutboundStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Outbound_Emergency')
    CREATE NONCLUSTERED INDEX IDX_Wms_Outbound_Emergency ON Wms_Outbound_OutboundOrder (IsEmergency, OutboundStatus) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 7: BC-06 Transfer Context (调拨)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Transfer_TransferOrder')
BEGIN
CREATE TABLE Wms_Transfer_TransferOrder (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TransferOrderNo     NVARCHAR(50)        NOT NULL,
    TransferType        INT                 NOT NULL,
    TransferStatus      INT                 NOT NULL DEFAULT 0,
    SourceWarehouseId   UNIQUEIDENTIFIER    NOT NULL,
    SourceWarehouseCode NVARCHAR(50)        NOT NULL,
    TargetWarehouseId   UNIQUEIDENTIFIER    NOT NULL,
    TargetWarehouseCode NVARCHAR(50)        NOT NULL,
    ApprovalStatus      INT                 NOT NULL DEFAULT 0,
    IsCrossCompany      BIT                 NOT NULL DEFAULT 0,
    Remark              NVARCHAR(1000)      NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Transfer_TransferOrder PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Transfer_TransferOrderNo UNIQUE (TransferOrderNo)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Transfer_TransferLine')
BEGIN
CREATE TABLE Wms_Transfer_TransferLine (
    Id                          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TransferOrderId             UNIQUEIDENTIFIER    NOT NULL,
    LineNo                      INT                 NOT NULL,
    MaterialId                  UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode                NVARCHAR(50)        NOT NULL,
    TransferQuantity            DECIMAL(18,4)       NOT NULL DEFAULT 0,
    OutboundConfirmedQuantity   DECIMAL(18,4)       NOT NULL DEFAULT 0,
    InboundConfirmedQuantity    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2               NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER        NULL,
    LastModificationTime    DATETIME2               NULL,
    LastModifierId          UNIQUEIDENTIFIER        NULL,
    IsDeleted               BIT                     NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER        NULL,
    DeletionTime            DATETIME2               NULL,
    CONSTRAINT PK_Wms_Transfer_TransferLine PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Transfer_Line_Composite UNIQUE (TransferOrderId, LineNo)
);
END
GO

-- Transfer 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Transfer_Status')
    CREATE NONCLUSTERED INDEX IDX_Wms_Transfer_Status ON Wms_Transfer_TransferOrder (TransferStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Transfer_SourceWarehouse')
    CREATE NONCLUSTERED INDEX IDX_Wms_Transfer_SourceWarehouse ON Wms_Transfer_TransferOrder (SourceWarehouseId) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 8: BC-07 CycleCount Context (盘点)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_CycleCount_CycleCountPlan')
BEGIN
CREATE TABLE Wms_CycleCount_CycleCountPlan (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PlanNo              NVARCHAR(50)        NOT NULL,
    CountMethod         INT                 NOT NULL,
    CountStatus         INT                 NOT NULL DEFAULT 0,
    WarehouseId         UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode       NVARCHAR(50)        NOT NULL,
    PlannedDate         DATETIME2           NOT NULL,
    FreezeInventory     BIT                 NOT NULL DEFAULT 0,
    DifferenceThreshold DECIMAL(18,4)       NOT NULL DEFAULT 0,
    BlindCountEnabled   BIT                 NOT NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_CycleCount_Plan PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_CycleCount_PlanNo UNIQUE (PlanNo)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_CycleCount_CycleCountItem')
BEGIN
CREATE TABLE Wms_CycleCount_CycleCountItem (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PlanId          UNIQUEIDENTIFIER    NOT NULL,
    LocationId      UNIQUEIDENTIFIER    NOT NULL,
    LocationCode    NVARCHAR(50)        NOT NULL,
    MaterialId      UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode    NVARCHAR(50)        NOT NULL,
    BatchNumber     NVARCHAR(50)        NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2   NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2   NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT         NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2   NULL,
    CONSTRAINT PK_Wms_CycleCount_Item PRIMARY KEY CLUSTERED (Id)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_CycleCount_CycleCountResult')
BEGIN
CREATE TABLE Wms_CycleCount_CycleCountResult (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PlanId              UNIQUEIDENTIFIER    NOT NULL,
    LocationId          UNIQUEIDENTIFIER    NOT NULL,
    LocationCode        NVARCHAR(50)        NOT NULL,
    MaterialId          UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode        NVARCHAR(50)        NOT NULL,
    SystemQuantity      DECIMAL(18,4)       NOT NULL DEFAULT 0,
    ActualQuantity      DECIMAL(18,4)       NOT NULL DEFAULT 0,
    DifferenceQuantity  DECIMAL(18,4)       NOT NULL DEFAULT 0,
    DifferenceAmount    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    BlindCountFlag      BIT                 NOT NULL DEFAULT 0,
    ResultStatus        INT                 NOT NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_CycleCount_Result PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- CycleCount 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_CycleCount_Status')
    CREATE NONCLUSTERED INDEX IDX_Wms_CycleCount_Status ON Wms_CycleCount_CycleCountPlan (CountStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_CycleCount_Warehouse')
    CREATE NONCLUSTERED INDEX IDX_Wms_CycleCount_Warehouse ON Wms_CycleCount_CycleCountPlan (WarehouseId) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 9: BC-08 LineSide Context (线边仓)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_LineSide_LineSideWarehouse')
BEGIN
CREATE TABLE Wms_LineSide_LineSideWarehouse (
    Id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    LineSideWarehouseCode   NVARCHAR(50)        NOT NULL,
    LineSideWarehouseName   NVARCHAR(200)       NOT NULL,
    WarehouseId             UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode           NVARCHAR(50)        NOT NULL,
    ProductionLineId        UNIQUEIDENTIFIER    NOT NULL,
    ProductionLineName      NVARCHAR(100)       NOT NULL,
    WorkStationId           UNIQUEIDENTIFIER    NOT NULL,
    ConsumptionMode         INT                 NOT NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER    NULL,
    LastModificationTime    DATETIME2           NULL,
    LastModifierId          UNIQUEIDENTIFIER    NULL,
    IsDeleted               BIT                 NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER    NULL,
    DeletionTime            DATETIME2           NULL,
    CONSTRAINT PK_Wms_LineSide_Warehouse PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_LineSide_WarehouseCode UNIQUE (LineSideWarehouseCode)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_LineSide_LineSideKanbanItem')
BEGIN
CREATE TABLE Wms_LineSide_LineSideKanbanItem (
    Id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    LineSideWarehouseId     UNIQUEIDENTIFIER    NOT NULL,
    MaterialId              UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode            NVARCHAR(50)        NOT NULL,
    MinQuantity             DECIMAL(18,4)       NOT NULL DEFAULT 0,
    MaxQuantity             DECIMAL(18,4)       NOT NULL DEFAULT 0,
    CurrentQuantity         DECIMAL(18,4)       NOT NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER    NULL,
    LastModificationTime    DATETIME2           NULL,
    LastModifierId          UNIQUEIDENTIFIER    NULL,
    IsDeleted               BIT                 NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER    NULL,
    DeletionTime            DATETIME2           NULL,
    CONSTRAINT PK_Wms_LineSide_KanbanItem PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- LineSide 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_LineSide_ProductionLine')
    CREATE NONCLUSTERED INDEX IDX_Wms_LineSide_ProductionLine ON Wms_LineSide_LineSideWarehouse (ProductionLineId) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 10: BC-09 Production Context (生产协同)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Production_ProductionOrder')
BEGIN
CREATE TABLE Wms_Production_ProductionOrder (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    ProductionOrderNo   NVARCHAR(50)        NOT NULL,
    ProductionStatus    INT                 NOT NULL DEFAULT 0,
    WarehouseId         UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode       NVARCHAR(50)        NOT NULL,
    MaterialId          UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode        NVARCHAR(50)        NOT NULL,
    PlanQuantity        DECIMAL(18,4)       NOT NULL DEFAULT 0,
    CompletedQuantity   DECIMAL(18,4)       NOT NULL DEFAULT 0,
    PlannedStartDate    DATETIME2           NULL,
    PlannedEndDate      DATETIME2           NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Production_Order PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Production_OrderNo UNIQUE (ProductionOrderNo)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Production_MaterialRequisition')
BEGIN
CREATE TABLE Wms_Production_MaterialRequisition (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    RequisitionNo       NVARCHAR(50)        NOT NULL,
    ProductionOrderId   UNIQUEIDENTIFIER    NOT NULL,
    ProductionOrderNo   NVARCHAR(50)        NOT NULL,
    RequisitionStatus   INT                 NOT NULL DEFAULT 0,
    WarehouseId         UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode       NVARCHAR(50)        NOT NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Production_Requisition PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Production_RequisitionNo UNIQUE (RequisitionNo)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Production_MaterialRequisitionLine')
BEGIN
CREATE TABLE Wms_Production_MaterialRequisitionLine (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    RequisitionId       UNIQUEIDENTIFIER    NOT NULL,
    LineNo              INT                 NOT NULL,
    MaterialId          UNIQUEIDENTIFIER    NOT NULL,
    MaterialCode        NVARCHAR(50)        NOT NULL,
    RequiredQuantity    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IssuedQuantity      DECIMAL(18,4)       NOT NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Production_RequisitionLine PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Production_ReqLine_Composite UNIQUE (RequisitionId, LineNo)
);
END
GO

-- Production 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Production_Status')
    CREATE NONCLUSTERED INDEX IDX_Wms_Production_Status ON Wms_Production_ProductionOrder (ProductionStatus) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 11: BC-10 TaskCenter Context (任务中心)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_TaskCenter_WarehouseTask')
BEGIN
CREATE TABLE Wms_TaskCenter_WarehouseTask (
    Id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TaskNo                  NVARCHAR(50)        NOT NULL,
    TaskType                INT                 NOT NULL,
    TaskPriority            INT                 NOT NULL DEFAULT 2,
    TaskStatus              INT                 NOT NULL DEFAULT 0,
    SourceOrderType         NVARCHAR(50)        NOT NULL,
    SourceOrderId           UNIQUEIDENTIFIER    NOT NULL,
    SourceOrderNo           NVARCHAR(50)        NOT NULL,
    WarehouseId             UNIQUEIDENTIFIER    NOT NULL,
    WarehouseCode           NVARCHAR(50)        NOT NULL,
    AssignedUserId          UNIQUEIDENTIFIER    NULL,
    AssignedUserName        NVARCHAR(100)       NULL,
    AssignmentStrategy      INT                 NOT NULL DEFAULT 0,
    ExpectedCompletionTime  DATETIME2           NULL,
    ActualStartTime         DATETIME2           NULL,
    ActualCompletionTime    DATETIME2           NULL,
    SuspendedReason         NVARCHAR(500)       NULL,
    TaskProgress            DECIMAL(5,2)        NOT NULL DEFAULT 0,
    Remark                  NVARCHAR(1000)      NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER    NULL,
    LastModificationTime    DATETIME2           NULL,
    LastModifierId          UNIQUEIDENTIFIER    NULL,
    IsDeleted               BIT                 NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER    NULL,
    DeletionTime            DATETIME2           NULL,
    CONSTRAINT PK_Wms_TaskCenter_WarehouseTask PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_TaskCenter_TaskNo UNIQUE (TaskNo)
);
END
GO

-- TaskCenter 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Task_WarehouseStatus')
    CREATE NONCLUSTERED INDEX IDX_Wms_Task_WarehouseStatus ON Wms_TaskCenter_WarehouseTask (WarehouseId, TaskStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Task_AssignedUser')
    CREATE NONCLUSTERED INDEX IDX_Wms_Task_AssignedUser ON Wms_TaskCenter_WarehouseTask (AssignedUserId, TaskStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Task_SourceOrder')
    CREATE NONCLUSTERED INDEX IDX_Wms_Task_SourceOrder ON Wms_TaskCenter_WarehouseTask (SourceOrderType, SourceOrderId) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Task_Priority')
    CREATE NONCLUSTERED INDEX IDX_Wms_Task_Priority ON Wms_TaskCenter_WarehouseTask (TaskPriority DESC, TaskStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Task_ExpectedTime')
    CREATE NONCLUSTERED INDEX IDX_Wms_Task_ExpectedTime ON Wms_TaskCenter_WarehouseTask (ExpectedCompletionTime, TaskStatus) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 12: BC-11 BarcodeLabel Context (条码标签)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_BarcodeLabel_BarcodeRule')
BEGIN
CREATE TABLE Wms_BarcodeLabel_BarcodeRule (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    RuleName        NVARCHAR(100)       NOT NULL,
    BarcodeType     INT                 NOT NULL,
    BarcodeFormat   NVARCHAR(200)       NOT NULL,
    CodePattern     NVARCHAR(500)       NOT NULL,
    IsActive        BIT                 NOT NULL DEFAULT 1,
    -- ABP Audit Fields
    CreationTime            DATETIME2   NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2   NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT         NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2   NULL,
    CONSTRAINT PK_Wms_BarcodeLabel_Rule PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_BarcodeLabel_RuleName UNIQUE (RuleName)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_BarcodeLabel_LabelTemplate')
BEGIN
CREATE TABLE Wms_BarcodeLabel_LabelTemplate (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TemplateName        NVARCHAR(100)       NOT NULL,
    TemplateType        INT                 NOT NULL,
    TemplateContent     NVARCHAR(MAX)       NOT NULL,
    TemplateVersion     INT                 NOT NULL DEFAULT 1,
    IndustryStandard    NVARCHAR(100)       NULL,
    IsActive            BIT                 NOT NULL DEFAULT 1,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_BarcodeLabel_Template PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_BarcodeLabel_TemplateName UNIQUE (TemplateName)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_BarcodeLabel_PrintJob')
BEGIN
CREATE TABLE Wms_BarcodeLabel_PrintJob (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PrintJobNo      NVARCHAR(50)        NULL,
    PrintJobStatus  INT                 NOT NULL DEFAULT 0,
    TemplateId      UNIQUEIDENTIFIER    NOT NULL,
    TemplateName    NVARCHAR(100)       NOT NULL,
    PrinterId       UNIQUEIDENTIFIER    NOT NULL,
    PrinterName     NVARCHAR(100)       NOT NULL,
    PrintContent    NVARCHAR(MAX)       NULL,
    TriggerSource   NVARCHAR(50)        NULL,
    SourceOrderId   UNIQUEIDENTIFIER    NULL,
    RetryCount      INT                 NOT NULL DEFAULT 0,
    -- ABP Audit Fields
    CreationTime            DATETIME2   NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2   NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT         NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2   NULL,
    CONSTRAINT PK_Wms_BarcodeLabel_PrintJob PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- BarcodeLabel 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_BarcodeRule_Type')
    CREATE NONCLUSTERED INDEX IDX_Wms_BarcodeRule_Type ON Wms_BarcodeLabel_BarcodeRule (BarcodeType) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 13: BC-12 Workflow Context (工作流)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Workflow_WorkflowDefinition')
BEGIN
CREATE TABLE Wms_Workflow_WorkflowDefinition (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    FlowName            NVARCHAR(100)       NOT NULL,
    FlowType            INT                 NOT NULL,
    IsActive            BIT                 NOT NULL DEFAULT 1,
    FlowDefinition      NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Workflow_Definition PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Workflow_FlowName UNIQUE (FlowName)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Workflow_WorkflowInstance')
BEGIN
CREATE TABLE Wms_Workflow_WorkflowInstance (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    FlowId              UNIQUEIDENTIFIER    NOT NULL,
    FlowName            NVARCHAR(100)       NOT NULL,
    InstanceStatus      INT                 NOT NULL DEFAULT 0,
    BusinessOrderId     UNIQUEIDENTIFIER    NOT NULL,
    BusinessOrderType   NVARCHAR(50)        NOT NULL,
    CurrentNodeId       UNIQUEIDENTIFIER    NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Workflow_Instance PRIMARY KEY CLUSTERED (Id)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Workflow_ApprovalActionLog')
BEGIN
CREATE TABLE Wms_Workflow_ApprovalActionLog (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    InstanceId      UNIQUEIDENTIFIER    NOT NULL,
    NodeId          UNIQUEIDENTIFIER    NOT NULL,
    ApproverId      UNIQUEIDENTIFIER    NOT NULL,
    ApproverName    NVARCHAR(100)       NOT NULL,
    ActionType      INT                 NOT NULL,
    Comment         NVARCHAR(500)       NULL,
    ActionTime      DATETIME2           NOT NULL DEFAULT GETDATE(),
    -- ABP Audit Fields
    CreationTime            DATETIME2   NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2   NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT         NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2   NULL,
    CONSTRAINT PK_Wms_Workflow_ApprovalLog PRIMARY KEY CLUSTERED (Id)
);
END
GO


-- ============================================================================
-- PART 14: BC-13 RuleEngine Context (规则引擎)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_RuleEngine_BusinessRule')
BEGIN
CREATE TABLE Wms_RuleEngine_BusinessRule (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    RuleName            NVARCHAR(100)       NOT NULL,
    RuleType            INT                 NOT NULL,
    RuleCondition       NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    RuleAction          NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    RuleVersion         INT                 NOT NULL DEFAULT 1,
    EffectiveStatus     BIT                 NOT NULL DEFAULT 0,
    EffectiveStartTime  DATETIME2           NULL,
    EffectiveEndTime    DATETIME2           NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_RuleEngine_BusinessRule PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_RuleEngine_RuleName UNIQUE (RuleName)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_RuleEngine_IndustryPackage')
BEGIN
CREATE TABLE Wms_RuleEngine_IndustryPackage (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PackageName     NVARCHAR(100)       NOT NULL,
    PackageVersion  NVARCHAR(50)        NOT NULL,
    PackageContent  NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    IndustryType    INT                 NOT NULL,
    Description     NVARCHAR(500)       NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2   NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2   NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT         NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2   NULL,
    CONSTRAINT PK_Wms_RuleEngine_IndustryPackage PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_RuleEngine_PackageName UNIQUE (PackageName)
);
END
GO


-- ============================================================================
-- PART 15: BC-14 Notification Context (通知)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Notification_NotificationTemplate')
BEGIN
CREATE TABLE Wms_Notification_NotificationTemplate (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TemplateName        NVARCHAR(100)       NOT NULL,
    TemplateContent     NVARCHAR(MAX)       NOT NULL,
    TemplateVariables   NVARCHAR(MAX)       NULL DEFAULT '{}',
    NotificationChannel INT                 NOT NULL DEFAULT 0,
    IsActive            BIT                 NOT NULL DEFAULT 1,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Notification_Template PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Notification_TemplateName UNIQUE (TemplateName)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Notification_NotificationLog')
BEGIN
CREATE TABLE Wms_Notification_NotificationLog (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    NotificationType    INT                 NOT NULL,
    Channel             INT                 NOT NULL DEFAULT 0,
    Title               NVARCHAR(200)       NOT NULL,
    Content             NVARCHAR(MAX)       NOT NULL,
    RecipientId         UNIQUEIDENTIFIER    NOT NULL,
    RecipientName       NVARCHAR(100)       NOT NULL,
    SendStatus          INT                 NOT NULL DEFAULT 0,
    SendTime            DATETIME2           NOT NULL DEFAULT GETDATE(),
    ErrorMessage        NVARCHAR(500)       NULL,
    TemplateId          UNIQUEIDENTIFIER    NULL,
    SourceEvent         NVARCHAR(100)       NULL,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Notification_Log PRIMARY KEY CLUSTERED (Id)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wms_Notification_NotificationRule')
BEGIN
CREATE TABLE Wms_Notification_NotificationRule (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    RuleName            NVARCHAR(100)       NOT NULL,
    RuleCondition       NVARCHAR(MAX)       NOT NULL DEFAULT '{}',
    EventSubscription   NVARCHAR(200)       NULL,
    TargetRole          INT                 NULL,
    TargetChannel       INT                 NOT NULL DEFAULT 0,
    IsActive            BIT                 NOT NULL DEFAULT 1,
    -- ABP Audit Fields
    CreationTime            DATETIME2       NOT NULL DEFAULT GETDATE(),
    CreatorId               UNIQUEIDENTIFIER NULL,
    LastModificationTime    DATETIME2       NULL,
    LastModifierId          UNIQUEIDENTIFIER NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,
    DeleterId               UNIQUEIDENTIFIER NULL,
    DeletionTime            DATETIME2       NULL,
    CONSTRAINT PK_Wms_Notification_Rule PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Wms_Notification_RuleName UNIQUE (RuleName)
);
END
GO

-- Notification 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Notification_Recipient')
    CREATE NONCLUSTERED INDEX IDX_Wms_Notification_Recipient ON Wms_Notification_NotificationLog (RecipientId) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Notification_Status')
    CREATE NONCLUSTERED INDEX IDX_Wms_Notification_Status ON Wms_Notification_NotificationLog (SendStatus) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_Wms_Notification_SendTime')
    CREATE NONCLUSTERED INDEX IDX_Wms_Notification_SendTime ON Wms_Notification_NotificationLog (SendTime DESC) WHERE IsDeleted = 0;
GO


-- ============================================================================
-- PART 16: 分区策略
-- ============================================================================

-- 16.1 InventoryLedgerEntry 按 OperationTime 月分区
IF NOT EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'PF_IV_Ledger_Monthly')
BEGIN
    CREATE PARTITION FUNCTION PF_IV_Ledger_Monthly (DATETIME2)
    AS RANGE RIGHT FOR VALUES (
        '2025-08-01','2025-09-01','2025-10-01','2025-11-01','2025-12-01',
        '2026-01-01','2026-02-01','2026-03-01','2026-04-01','2026-05-01',
        '2026-06-01','2026-07-01','2026-08-01','2026-09-01','2026-10-01',
        '2026-11-01','2026-12-01'
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'PS_IV_Ledger_Monthly')
BEGIN
    CREATE PARTITION SCHEME PS_IV_Ledger_Monthly
    AS PARTITION PF_IV_Ledger_Monthly ALL TO ([PRIMARY]);
END
GO

-- 16.2 NotificationLog 按 CreationTime 月分区
IF NOT EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'PF_NT_Log_Monthly')
BEGIN
    CREATE PARTITION FUNCTION PF_NT_Log_Monthly (DATETIME2)
    AS RANGE RIGHT FOR VALUES (
        '2025-08-01','2025-09-01','2025-10-01','2025-11-01','2025-12-01',
        '2026-01-01','2026-02-01','2026-03-01','2026-04-01','2026-05-01',
        '2026-06-01','2026-07-01','2026-08-01','2026-09-01','2026-10-01',
        '2026-11-01','2026-12-01'
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'PS_NT_Log_Monthly')
BEGIN
    CREATE PARTITION SCHEME PS_NT_Log_Monthly
    AS PARTITION PF_NT_Log_Monthly ALL TO ([PRIMARY]);
END
GO


-- ============================================================================
-- PART 17: 数据库角色与权限
-- ============================================================================

-- 17.1 创建应用程序数据库角色
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'Wms_AppRole' AND type = 'R')
BEGIN
    CREATE ROLE Wms_AppRole;
END
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'Wms_ReadOnlyRole' AND type = 'R')
BEGIN
    CREATE ROLE Wms_ReadOnlyRole;
END
GO

-- 17.2 授予 Wms_AppRole 数据操作权限
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO Wms_AppRole;
GO

-- 17.3 库存台账特殊权限 — 禁止修改和删除
DENY UPDATE, DELETE ON Wms_Inventory_InventoryLedgerEntry TO Wms_AppRole;
GO

-- 17.4 授予 Wms_ReadOnlyRole 只读权限
GRANT SELECT ON SCHEMA::dbo TO Wms_ReadOnlyRole;
GO


-- ============================================================================
-- PART 18: 种子数据 — 基本信息
-- ============================================================================

PRINT '>>> 开始插入种子数据...';

-- 18.1 默认角色
DECLARE @AdminRoleId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @OperatorRoleId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @ViewerRoleId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @SystemUserId UNIQUEIDENTIFIER = 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA';

IF NOT EXISTS (SELECT 1 FROM AbpRoles WHERE Name = 'admin')
BEGIN
    INSERT INTO AbpRoles (Id, Name, NormalizedName, ConcurrencyStamp, IsDefault, IsPublic, IsStatic, CreationTime)
    VALUES (@AdminRoleId, 'admin', 'ADMIN', NEWID(), 1, 0, 1, GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM AbpRoles WHERE Name = 'operator')
BEGIN
    INSERT INTO AbpRoles (Id, Name, NormalizedName, ConcurrencyStamp, IsDefault, IsPublic, IsStatic, CreationTime)
    VALUES (@OperatorRoleId, 'operator', 'OPERATOR', NEWID(), 1, 0, 1, GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM AbpRoles WHERE Name = 'viewer')
BEGIN
    INSERT INTO AbpRoles (Id, Name, NormalizedName, ConcurrencyStamp, IsDefault, IsPublic, IsStatic, CreationTime)
    VALUES (@ViewerRoleId, 'viewer', 'VIEWER', NEWID(), 1, 0, 1, GETDATE());
END
GO

-- 18.2 默认组织单元
DECLARE @OrgRootId UNIQUEIDENTIFIER = 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB';
DECLARE @OrgFactoryId UNIQUEIDENTIFIER = 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC';
DECLARE @OrgWarehouseId UNIQUEIDENTIFIER = 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD';

IF NOT EXISTS (SELECT 1 FROM AbpOrganizationUnits WHERE Code = 'ROOT')
BEGIN
    INSERT INTO AbpOrganizationUnits (Id, ParentId, Code, DisplayName, CreationTime)
    VALUES (@OrgRootId, NULL, 'ROOT', '总公司', GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM AbpOrganizationUnits WHERE Code = 'FACTORY_01')
BEGIN
    INSERT INTO AbpOrganizationUnits (Id, ParentId, Code, DisplayName, CreationTime)
    VALUES (@OrgFactoryId, @OrgRootId, 'FACTORY_01', '一厂', GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM AbpOrganizationUnits WHERE Code = 'WH_01')
BEGIN
    INSERT INTO AbpOrganizationUnits (Id, ParentId, Code, DisplayName, CreationTime)
    VALUES (@OrgWarehouseId, @OrgFactoryId, 'WH_01', '仓储中心', GETDATE());
END
GO

-- 18.3 默认系统用户 (Password: Abc123!@#)
-- 密码哈希: ABP默认PBKDF2, 此处为预计算值
DECLARE @AdminUserId UNIQUEIDENTIFIER = 'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF';
DECLARE @OperatorUserId UNIQUEIDENTIFIER = 'EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE';

IF NOT EXISTS (SELECT 1 FROM AbpUsers WHERE UserName = 'admin')
BEGIN
    INSERT INTO AbpUsers (Id, UserName, NormalizedUserName, Name, Surname, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, IsActive, CreationTime)
    VALUES (@AdminUserId, 'admin', 'ADMIN', '系统', '管理员', 'admin@wms.com', 'ADMIN@WMS.COM', 1,
        'AQAAAAIAAYagAAAAEKV7gJv8f7xHhZLp3qJJ5+2R+X8CPqKx3NZwF7wFmmSw==', -- 预计算密码哈希
        NEWID(), NEWID(), 0, 0, 1, 1, GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM AbpUsers WHERE UserName = 'operator')
BEGIN
    INSERT INTO AbpUsers (Id, UserName, NormalizedUserName, Name, Surname, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, IsActive, CreationTime)
    VALUES (@OperatorUserId, 'operator', 'OPERATOR', '仓库', '操作员', 'operator@wms.com', 'OPERATOR@WMS.COM', 1,
        'AQAAAAIAAYagAAAAEKV7gJv8f7xHhZLp3qJJ5+2R+X8CPqKx3NZwF7wFmmSw==',
        NEWID(), NEWID(), 0, 0, 1, 1, GETDATE());
END
GO

-- 18.4 用户角色关联
IF NOT EXISTS (SELECT 1 FROM AbpUserRoles WHERE UserId = @AdminUserId AND RoleId = @AdminRoleId)
    INSERT INTO AbpUserRoles (UserId, RoleId) VALUES (@AdminUserId, @AdminRoleId);
GO
IF NOT EXISTS (SELECT 1 FROM AbpUserRoles WHERE UserId = @OperatorUserId AND RoleId = @OperatorRoleId)
    INSERT INTO AbpUserRoles (UserId, RoleId) VALUES (@OperatorUserId, @OperatorRoleId);
GO

-- 18.5 种子数据 — 计量单位 (MIG-004: 20+基础单位)
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'PCS')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'PCS', '个/件', 'pcs', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'KG')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'KG', '千克', 'kg', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'G')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'G', '克', 'g', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'TON')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'TON', '吨', 't', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'M')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'M', '米', 'm', 2, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'CM')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'CM', '厘米', 'cm', 2, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'MM')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'MM', '毫米', 'mm', 2, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'L')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'L', '升', 'L', 3, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'ML')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'ML', '毫升', 'mL', 3, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'M2')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'M2', '平方米', 'm²', 4, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'BOX')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'BOX', '盒', 'box', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'BAG')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'BAG', '袋', 'bag', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'ROLL')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'ROLL', '卷', 'roll', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'SET')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'SET', '套', 'set', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'PAL')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'PAL', '托盘', 'pal', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'CARTON')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'CARTON', '箱', 'ctn', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'BOTTLE')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'BOTTLE', '瓶', 'btl', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'PAIR')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'PAIR', '双/对', 'pr', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'SHEET')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'SHEET', '张', 'sht', 0, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_UnitOfMeasure WHERE UnitCode = 'BUNDLE')
    INSERT INTO Wms_Material_UnitOfMeasure (Id, UnitCode, UnitName, UnitSymbol, UnitType, CreationTime)
    VALUES (NEWID(), 'BUNDLE', '捆', 'bdl', 0, GETDATE());
GO

-- 18.6 种子数据 — 物料分类
DECLARE @RawMatClassId UNIQUEIDENTIFIER = NEWID();
DECLARE @SemiFinishedClassId UNIQUEIDENTIFIER = NEWID();
DECLARE @FinishedClassId UNIQUEIDENTIFIER = NEWID();
DECLARE @PackagingClassId UNIQUEIDENTIFIER = NEWID();
DECLARE @AuxiliaryClassId UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT 1 FROM Wms_Material_MaterialClassification WHERE ClassificationCode = 'MAT-RAW')
    INSERT INTO Wms_Material_MaterialClassification (Id, ClassificationCode, ClassificationName, ClassificationLevel, CreationTime)
    VALUES (@RawMatClassId, 'MAT-RAW', '原材料', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_MaterialClassification WHERE ClassificationCode = 'MAT-SEMI')
    INSERT INTO Wms_Material_MaterialClassification (Id, ClassificationCode, ClassificationName, ClassificationLevel, CreationTime)
    VALUES (@SemiFinishedClassId, 'MAT-SEMI', '半成品', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_MaterialClassification WHERE ClassificationCode = 'MAT-FG')
    INSERT INTO Wms_Material_MaterialClassification (Id, ClassificationCode, ClassificationName, ClassificationLevel, CreationTime)
    VALUES (@FinishedClassId, 'MAT-FG', '成品', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_MaterialClassification WHERE ClassificationCode = 'MAT-PKG')
    INSERT INTO Wms_Material_MaterialClassification (Id, ClassificationCode, ClassificationName, ClassificationLevel, CreationTime)
    VALUES (@PackagingClassId, 'MAT-PKG', '包装材料', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Material_MaterialClassification WHERE ClassificationCode = 'MAT-AUX')
    INSERT INTO Wms_Material_MaterialClassification (Id, ClassificationCode, ClassificationName, ClassificationLevel, CreationTime)
    VALUES (@AuxiliaryClassId, 'MAT-AUX', '辅助材料', 1, GETDATE());
GO

-- 18.7 种子数据 — 示例仓库 (MIG-009 Demo数据)
DECLARE @DemoWarehouseId UNIQUEIDENTIFIER = NEWID();
DECLARE @DemoPlantId UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Warehouse WHERE WarehouseCode = 'WH-MAIN-01')
BEGIN
    INSERT INTO Wms_Warehouse_Warehouse (Id, WarehouseCode, WarehouseName, WarehouseType,
        OrganizationUnitId, OrganizationUnitName, PlantId, PlantName,
        ResponsibleUserName, Address, StorageConditionType, LocationLevelCount, IsActive, Remark, CreationTime)
    VALUES (@DemoWarehouseId, 'WH-MAIN-01', '一号主仓库', 0,
        @OrgWarehouseId, '仓储中心', @DemoPlantId, '一厂',
        '系统管理员', '广东省深圳市宝安区XX路100号', 0, 3, 1,
        '系统初始化默认仓库，可手动修改', GETDATE());
END
GO

-- 18.8 种子数据 — 示例库区
DECLARE @DemoAreaReceiveId UNIQUEIDENTIFIER = NEWID();
DECLARE @DemoAreaStorageId UNIQUEIDENTIFIER = NEWID();
DECLARE @DemoAreaShipId UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_WarehouseArea WHERE AreaCode = 'WH-MAIN-01-RCV' AND WarehouseId = @DemoWarehouseId)
BEGIN
    INSERT INTO Wms_Warehouse_WarehouseArea (Id, AreaCode, AreaName, WarehouseId, WarehouseCode, AreaFunction, StorageEnvironment, MaxCapacity, CreationTime)
    VALUES (@DemoAreaReceiveId, 'WH-MAIN-01-RCV', '收货区', @DemoWarehouseId, 'WH-MAIN-01', 0, 0, 5000.0000, GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_WarehouseArea WHERE AreaCode = 'WH-MAIN-01-STO' AND WarehouseId = @DemoWarehouseId)
BEGIN
    INSERT INTO Wms_Warehouse_WarehouseArea (Id, AreaCode, AreaName, WarehouseId, WarehouseCode, AreaFunction, StorageEnvironment, MaxCapacity, CreationTime)
    VALUES (@DemoAreaStorageId, 'WH-MAIN-01-STO', '存储区', @DemoWarehouseId, 'WH-MAIN-01', 1, 0, 50000.0000, GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_WarehouseArea WHERE AreaCode = 'WH-MAIN-01-SHP' AND WarehouseId = @DemoWarehouseId)
BEGIN
    INSERT INTO Wms_Warehouse_WarehouseArea (Id, AreaCode, AreaName, WarehouseId, WarehouseCode, AreaFunction, StorageEnvironment, MaxCapacity, CreationTime)
    VALUES (@DemoAreaShipId, 'WH-MAIN-01-SHP', '发货区', @DemoWarehouseId, 'WH-MAIN-01', 2, 0, 3000.0000, GETDATE());
END
GO

-- 18.9 种子数据 — 示例库位 (每个库区4个)
DECLARE @LocPrefix NVARCHAR(20) = 'WH-MAIN-01';
DECLARE @DemoAreaReceiveId_loc UNIQUEIDENTIFIER = (SELECT Id FROM Wms_Warehouse_WarehouseArea WHERE AreaCode = 'WH-MAIN-01-RCV' AND WarehouseId = @DemoWarehouseId);
DECLARE @DemoAreaStorageId_loc UNIQUEIDENTIFIER = (SELECT Id FROM Wms_Warehouse_WarehouseArea WHERE AreaCode = 'WH-MAIN-01-STO' AND WarehouseId = @DemoWarehouseId);
DECLARE @DemoAreaShipId_loc UNIQUEIDENTIFIER = (SELECT Id FROM Wms_Warehouse_WarehouseArea WHERE AreaCode = 'WH-MAIN-01-SHP' AND WarehouseId = @DemoWarehouseId);

-- 收货区库位
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-RCV-A01')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-RCV-A01', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaReceiveId_loc, 'WH-MAIN-01-RCV', 0, 2000.0000, 10.0000, 'LOC-RCV-A01', 'A', '01', '0', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-RCV-A02')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-RCV-A02', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaReceiveId_loc, 'WH-MAIN-01-RCV', 0, 2000.0000, 10.0000, 'LOC-RCV-A02', 'A', '02', '0', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-RCV-B01')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-RCV-B01', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaReceiveId_loc, 'WH-MAIN-01-RCV', 0, 2000.0000, 10.0000, 'LOC-RCV-B01', 'B', '01', '0', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-RCV-B02')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-RCV-B02', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaReceiveId_loc, 'WH-MAIN-01-RCV', 0, 2000.0000, 10.0000, 'LOC-RCV-B02', 'B', '02', '0', GETDATE());
GO

-- 存储区库位
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-STO-A01-01')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-STO-A01-01', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaStorageId_loc, 'WH-MAIN-01-STO', 1, 5000.0000, 50.0000, 'LOC-STO-A01-01', 'A', '01', '01', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-STO-A01-02')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-STO-A01-02', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaStorageId_loc, 'WH-MAIN-01-STO', 1, 5000.0000, 50.0000, 'LOC-STO-A01-02', 'A', '01', '02', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-STO-B01-01')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-STO-B01-01', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaStorageId_loc, 'WH-MAIN-01-STO', 1, 5000.0000, 50.0000, 'LOC-STO-B01-01', 'B', '01', '01', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-STO-B01-02')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-STO-B01-02', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaStorageId_loc, 'WH-MAIN-01-STO', 1, 5000.0000, 50.0000, 'LOC-STO-B01-02', 'B', '01', '02', GETDATE());
GO

-- 发货区库位
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-SHP-A01')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-SHP-A01', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaShipId_loc, 'WH-MAIN-01-SHP', 2, 3000.0000, 15.0000, 'LOC-SHP-A01', 'A', '01', '0', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-SHP-A02')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-SHP-A02', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaShipId_loc, 'WH-MAIN-01-SHP', 2, 3000.0000, 15.0000, 'LOC-SHP-A02', 'A', '02', '0', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-SHP-B01')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-SHP-B01', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaShipId_loc, 'WH-MAIN-01-SHP', 2, 3000.0000, 15.0000, 'LOC-SHP-B01', 'B', '01', '0', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Warehouse_Location WHERE LocationCode = @LocPrefix + '-SHP-B02')
    INSERT INTO Wms_Warehouse_Location (Id, LocationCode, WarehouseId, WarehouseCode, AreaId, AreaCode, LocationType, MaxWeight, MaxCapacity, BarcodeId, [Row], [Column], [Layer], CreationTime)
    VALUES (NEWID(), @LocPrefix + '-SHP-B02', @DemoWarehouseId, 'WH-MAIN-01', @DemoAreaShipId_loc, 'WH-MAIN-01-SHP', 2, 3000.0000, 15.0000, 'LOC-SHP-B02', 'B', '02', '0', GETDATE());
GO

-- 18.10 种子数据 — 条码规则 (MIG-005)
IF NOT EXISTS (SELECT 1 FROM Wms_BarcodeLabel_BarcodeRule WHERE RuleName = '物料条码规则')
    INSERT INTO Wms_BarcodeLabel_BarcodeRule (Id, RuleName, BarcodeType, BarcodeFormat, CodePattern, IsActive, CreationTime)
    VALUES (NEWID(), '物料条码规则', 0, 'CODE128', 'MAT-{yyyyMMdd}-{0000}', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_BarcodeLabel_BarcodeRule WHERE RuleName = '库位条码规则')
    INSERT INTO Wms_BarcodeLabel_BarcodeRule (Id, RuleName, BarcodeType, BarcodeFormat, CodePattern, IsActive, CreationTime)
    VALUES (NEWID(), '库位条码规则', 0, 'CODE128', 'LOC-{WarehouseCode}-{Row}{Column}{Layer}', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_BarcodeLabel_BarcodeRule WHERE RuleName = '托盘条码规则')
    INSERT INTO Wms_BarcodeLabel_BarcodeRule (Id, RuleName, BarcodeType, BarcodeFormat, CodePattern, IsActive, CreationTime)
    VALUES (NEWID(), '托盘条码规则', 0, 'CODE128', 'PAL-{yyyyMMdd}-{0000}', 1, GETDATE());
GO

-- 18.11 种子数据 — 标签模板 (MIG-006)
IF NOT EXISTS (SELECT 1 FROM Wms_BarcodeLabel_LabelTemplate WHERE TemplateName = '入库标签模板')
    INSERT INTO Wms_BarcodeLabel_LabelTemplate (Id, TemplateName, TemplateType, TemplateContent, TemplateVersion, IndustryStandard, CreationTime)
    VALUES (NEWID(), '入库标签模板', 0,
        '{"size":"100x50mm","fields":[{"name":"MaterialCode","label":"物料编码","x":5,"y":5},{"name":"MaterialName","label":"物料名称","x":5,"y":15},{"name":"BatchNo","label":"批次号","x":5,"y":25},{"name":"Qty","label":"数量","x":5,"y":35},{"name":"Barcode","label":"条码","x":5,"y":40,"type":"barcode"}]}',
        1, 'GB/T 14258', GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_BarcodeLabel_LabelTemplate WHERE TemplateName = '出库标签模板')
    INSERT INTO Wms_BarcodeLabel_LabelTemplate (Id, TemplateName, TemplateType, TemplateContent, TemplateVersion, IndustryStandard, CreationTime)
    VALUES (NEWID(), '出库标签模板', 1,
        '{"size":"100x50mm","fields":[{"name":"OutOrderNo","label":"出库单号","x":5,"y":5},{"name":"MaterialCode","label":"物料编码","x":5,"y":15},{"name":"Qty","label":"数量","x":5,"y":25},{"name":"Location","label":"库位","x":5,"y":35},{"name":"Barcode","label":"条码","x":5,"y":40,"type":"barcode"}]}',
        1, 'GB/T 14258', GETDATE());
GO

-- 18.12 种子数据 — 通知规则 (MIG-007)
IF NOT EXISTS (SELECT 1 FROM Wms_Notification_NotificationRule WHERE RuleName = '安全库存预警')
    INSERT INTO Wms_Notification_NotificationRule (Id, RuleName, RuleCondition, EventSubscription, TargetRole, TargetChannel, IsActive, CreationTime)
    VALUES (NEWID(), '安全库存预警',
        '{"type":"SafetyStock","condition":"CurrentQuantity <= ThresholdQuantity"}',
        'Inventory.Alert.Created', 0, 1, 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Notification_NotificationRule WHERE RuleName = '临期物料预警')
    INSERT INTO Wms_Notification_NotificationRule (Id, RuleName, RuleCondition, EventSubscription, TargetRole, TargetChannel, IsActive, CreationTime)
    VALUES (NEWID(), '临期物料预警',
        '{"type":"Expiry","condition":"DaysUntilExpiry <= 30"}',
        'Inventory.Expiry.Alert', 0, 1, 1, GETDATE());
GO

-- 18.13 种子数据 — 通知模板
IF NOT EXISTS (SELECT 1 FROM Wms_Notification_NotificationTemplate WHERE TemplateName = '安全库存预警模板')
    INSERT INTO Wms_Notification_NotificationTemplate (Id, TemplateName, TemplateContent, TemplateVariables, NotificationChannel, CreationTime)
    VALUES (NEWID(), '安全库存预警模板',
        '物料 [{MaterialCode}] {MaterialName} 当前库存 {CurrentQuantity} 已低于安全库存阈值 {ThresholdQuantity}，请及时补货。',
        '["MaterialCode","MaterialName","CurrentQuantity","ThresholdQuantity","WarehouseCode"]', 1, GETDATE());
GO
IF NOT EXISTS (SELECT 1 FROM Wms_Notification_NotificationTemplate WHERE TemplateName = '临期预警模板')
    INSERT INTO Wms_Notification_NotificationTemplate (Id, TemplateName, TemplateContent, TemplateVariables, NotificationChannel, CreationTime)
    VALUES (NEWID(), '临期预警模板',
        '物料 [{MaterialCode}] {MaterialName} 批次 {BatchNumber} 将在 {DaysUntilExpiry} 天后过期（过期日: {ExpiryDate}），请及时处理。',
        '["MaterialCode","MaterialName","BatchNumber","ExpiryDate","DaysUntilExpiry","WarehouseCode"]', 1, GETDATE());
GO

PRINT '<<< 种子数据插入完成。';

-- ============================================================================
-- PART 19: 工作流定义种子数据
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM Wms_Workflow_WorkflowDefinition WHERE FlowName = '入库审批流程')
    INSERT INTO Wms_Workflow_WorkflowDefinition (Id, FlowName, FlowType, IsActive, FlowDefinition, CreationTime)
    VALUES (NEWID(), '入库审批流程', 0, 1,
        '{"nodes":[{"id":"start","type":"start"},{"id":"warehouse_verify","type":"task","assignee":"warehouse_manager"},{"id":"quality_check","type":"task","assignee":"quality_inspector"},{"id":"approve","type":"task","assignee":"supervisor"},{"id":"end","type":"end"}],"edges":[{"from":"start","to":"warehouse_verify"},{"from":"warehouse_verify","to":"quality_check"},{"from":"quality_check","to":"approve"},{"from":"approve","to":"end"}]}',
        GETDATE());
GO

IF NOT EXISTS (SELECT 1 FROM Wms_Workflow_WorkflowDefinition WHERE FlowName = '出库审批流程')
    INSERT INTO Wms_Workflow_WorkflowDefinition (Id, FlowName, FlowType, IsActive, FlowDefinition, CreationTime)
    VALUES (NEWID(), '出库审批流程', 1, 1,
        '{"nodes":[{"id":"start","type":"start"},{"id":"pick_verify","type":"task","assignee":"picker"},{"id":"quality_confirm","type":"task","assignee":"quality_inspector"},{"id":"ship_approve","type":"task","assignee":"supervisor"},{"id":"end","type":"end"}],"edges":[{"from":"start","to":"pick_verify"},{"from":"pick_verify","to":"quality_confirm"},{"from":"quality_confirm","to":"ship_approve"},{"from":"ship_approve","to":"end"}]}',
        GETDATE());
GO

-- ============================================================================
-- 完成标记
-- ============================================================================
PRINT '============================================================================';
PRINT ' Manufacturing WMS Database - 创建完成';
PRINT ' 数据库: WmsDb';
PRINT ' 表数量: 42+ (含ABP身份认证表)';
PRINT ' 种子数据: 角色/用户/组织单元/计量单位/分类/仓库/库区/库位/条码规则/标签模板/通知规则';
PRINT '============================================================================';
GO
