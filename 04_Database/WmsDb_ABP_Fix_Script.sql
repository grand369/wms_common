USE WmsDb;
GO

-- ============================================================================
-- ABP Identity 表缺失字段修复
-- ============================================================================

-- AbpUsers 表添加缺失字段
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'EntityVersion' AND object_id = OBJECT_ID('AbpUsers'))
BEGIN
    ALTER TABLE AbpUsers ADD EntityVersion INT NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'LastPasswordChangeTime' AND object_id = OBJECT_ID('AbpUsers'))
BEGIN
    ALTER TABLE AbpUsers ADD LastPasswordChangeTime DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ShouldChangePasswordOnNextLogin' AND object_id = OBJECT_ID('AbpUsers'))
BEGIN
    ALTER TABLE AbpUsers ADD ShouldChangePasswordOnNextLogin BIT NOT NULL DEFAULT 0;
END
GO

-- AbpRoles 表添加缺失字段
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'EntityVersion' AND object_id = OBJECT_ID('AbpRoles'))
BEGIN
    ALTER TABLE AbpRoles ADD EntityVersion INT NOT NULL DEFAULT 0;
END
GO

-- ============================================================================
-- ABP Identity 缺失表创建
-- ============================================================================

-- AbpUserClaims
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpUserClaims')
BEGIN
CREATE TABLE AbpUserClaims (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    UserId      UNIQUEIDENTIFIER    NOT NULL,
    ClaimType   NVARCHAR(256)       NULL,
    ClaimValue  NVARCHAR(MAX)       NULL,
    TenantId    UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpUserClaims PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_AbpUserClaims_UserId FOREIGN KEY (UserId) REFERENCES AbpUsers(Id) ON DELETE CASCADE
);
END
GO

-- AbpUserLogins
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpUserLogins')
BEGIN
CREATE TABLE AbpUserLogins (
    LoginProvider    NVARCHAR(128)       NOT NULL,
    ProviderKey      NVARCHAR(128)       NOT NULL,
    ProviderDisplayName NVARCHAR(256)    NULL,
    UserId          UNIQUEIDENTIFIER    NOT NULL,
    TenantId        UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpUserLogins PRIMARY KEY CLUSTERED (LoginProvider, ProviderKey),
    CONSTRAINT FK_AbpUserLogins_UserId FOREIGN KEY (UserId) REFERENCES AbpUsers(Id) ON DELETE CASCADE
);
END
GO

-- AbpUserTokens
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpUserTokens')
BEGIN
CREATE TABLE AbpUserTokens (
    UserId          UNIQUEIDENTIFIER    NOT NULL,
    LoginProvider   NVARCHAR(128)       NOT NULL,
    Name            NVARCHAR(128)       NOT NULL,
    Value           NVARCHAR(MAX)       NULL,
    TenantId        UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpUserTokens PRIMARY KEY CLUSTERED (UserId, LoginProvider, Name),
    CONSTRAINT FK_AbpUserTokens_UserId FOREIGN KEY (UserId) REFERENCES AbpUsers(Id) ON DELETE CASCADE
);
END
GO

-- AbpRoleClaims
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpRoleClaims')
BEGIN
CREATE TABLE AbpRoleClaims (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    RoleId      UNIQUEIDENTIFIER    NOT NULL,
    ClaimType   NVARCHAR(256)       NULL,
    ClaimValue  NVARCHAR(MAX)       NULL,
    TenantId    UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpRoleClaims PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_AbpRoleClaims_RoleId FOREIGN KEY (RoleId) REFERENCES AbpRoles(Id) ON DELETE CASCADE
);
END
GO

-- AbpUserDelegations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpUserDelegations')
BEGIN
CREATE TABLE AbpUserDelegations (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TenantId            UNIQUEIDENTIFIER    NULL,
    SourceUserId        UNIQUEIDENTIFIER    NOT NULL,
    TargetUserId        UNIQUEIDENTIFIER    NOT NULL,
    StartTime           DATETIME2           NOT NULL,
    EndTime             DATETIME2           NULL,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpUserDelegations PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- AbpClaimTypes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpClaimTypes')
BEGIN
CREATE TABLE AbpClaimTypes (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    Name                NVARCHAR(256)       NOT NULL,
    Required            BIT                 NOT NULL DEFAULT 0,
    IsStatic            BIT                 NOT NULL DEFAULT 0,
    RegularExpression   NVARCHAR(512)       NULL,
    Description         NVARCHAR(256)       NULL,
    ValidationError     NVARCHAR(256)       NULL,
    TenantId            UNIQUEIDENTIFIER    NULL,
    ExtraProperties     NVARCHAR(MAX)       NULL,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER    NULL,
    LastModificationTime DATETIME2          NULL,
    LastModifierId      UNIQUEIDENTIFIER    NULL,
    IsDeleted           BIT                 NOT NULL DEFAULT 0,
    DeleterId           UNIQUEIDENTIFIER    NULL,
    DeletionTime        DATETIME2           NULL,
    CONSTRAINT PK_AbpClaimTypes PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_AbpClaimTypes_Name UNIQUE (Name)
);
END
GO

-- AbpIdentityLinkUsers
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpIdentityLinkUsers')
BEGIN
CREATE TABLE AbpIdentityLinkUsers (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TenantId            UNIQUEIDENTIFIER    NULL,
    UserId              UNIQUEIDENTIFIER    NOT NULL,
    LinkedUserId        UNIQUEIDENTIFIER    NOT NULL,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpIdentityLinkUsers PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_AbpIdentityLinkUsers_UserLinkedUser UNIQUE (UserId, LinkedUserId)
);
END
GO

-- AbpIdentitySecurityLogs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpIdentitySecurityLogs')
BEGIN
CREATE TABLE AbpIdentitySecurityLogs (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    ApplicationName     NVARCHAR(128)       NULL,
    Identity            NVARCHAR(256)       NULL,
    Action              NVARCHAR(128)       NULL,
    UserId              UNIQUEIDENTIFIER    NULL,
    UserName            NVARCHAR(256)       NULL,
    ClientId            NVARCHAR(128)       NULL,
    ClientName          NVARCHAR(256)       NULL,
    CorrelationId       NVARCHAR(64)        NULL,
    ClientIpAddress     NVARCHAR(64)        NULL,
    ClientTenantId      UNIQUEIDENTIFIER    NULL,
    BrowserInfo         NVARCHAR(512)       NULL,
    Exception           NVARCHAR(MAX)       NULL,
    Success             BIT                 NOT NULL DEFAULT 1,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_AbpIdentitySecurityLogs PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- AbpIdentitySessions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpIdentitySessions')
BEGIN
CREATE TABLE AbpIdentitySessions (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TenantId            UNIQUEIDENTIFIER    NULL,
    UserId              UNIQUEIDENTIFIER    NULL,
    ApplicationName     NVARCHAR(128)       NULL,
    ClientIpAddress     NVARCHAR(64)        NULL,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    ExpireTime          DATETIME2           NULL,
    CONSTRAINT PK_AbpIdentitySessions PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- ============================================================================
-- ABP Permission Management 表
-- ============================================================================

-- AbpPermissionGrants
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpPermissionGrants')
BEGIN
CREATE TABLE AbpPermissionGrants (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    Name                NVARCHAR(256)       NOT NULL,
    ProviderName        NVARCHAR(64)        NOT NULL,
    ProviderKey         NVARCHAR(256)       NOT NULL,
    TenantId            UNIQUEIDENTIFIER    NULL,
    ExtraProperties     NVARCHAR(MAX)       NULL,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpPermissionGrants PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_AbpPermissionGrants_NameProvider UNIQUE (Name, ProviderName, ProviderKey)
);
END
GO

-- ============================================================================
-- ABP Audit Log 表
-- ============================================================================

-- AbpAuditLogs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpAuditLogs')
BEGIN
CREATE TABLE AbpAuditLogs (
    Id                  BIGINT              NOT NULL IDENTITY(1,1),
    TenantId            UNIQUEIDENTIFIER    NULL,
    ApplicationName     NVARCHAR(128)       NULL,
    UserId              UNIQUEIDENTIFIER    NULL,
    UserName            NVARCHAR(256)       NULL,
    ImpersonatorUserId  UNIQUEIDENTIFIER    NULL,
    ImpersonatorUserName NVARCHAR(256)      NULL,
    ClientId            NVARCHAR(64)        NULL,
    ClientName          NVARCHAR(128)       NULL,
    ClientIpAddress     NVARCHAR(64)        NULL,
    CorrelationId       NVARCHAR(64)        NULL,
    ExecutionTime       DATETIME2           NOT NULL DEFAULT GETDATE(),
    ExecutionDuration   INT                 NOT NULL DEFAULT 0,
    HttpMethod          NVARCHAR(16)        NULL,
    Url                 NVARCHAR(256)       NULL,
    ControllerName      NVARCHAR(128)       NULL,
    ActionName          NVARCHAR(128)       NULL,
    Exception           NVARCHAR(MAX)       NULL,
    Parameters          NVARCHAR(MAX)       NULL,
    Result              NVARCHAR(MAX)       NULL,
    IsSuccess           BIT                 NOT NULL DEFAULT 1,
    CONSTRAINT PK_AbpAuditLogs PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- ============================================================================
-- ABP Background Jobs 表
-- ============================================================================

-- AbpBackgroundJobs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpBackgroundJobs')
BEGIN
CREATE TABLE AbpBackgroundJobs (
    Id                  BIGINT              NOT NULL IDENTITY(1,1),
    JobName             NVARCHAR(256)       NOT NULL,
    JobArgs             NVARCHAR(MAX)       NOT NULL,
    Priority            INT                 NOT NULL DEFAULT 10,
    TryCount            INT                 NOT NULL DEFAULT 0,
    NextTryTime         DATETIME2           NOT NULL DEFAULT GETDATE(),
    LastTryTime         DATETIME2           NULL,
    LastTryMessage      NVARCHAR(MAX)       NULL,
    IsAbandoned         BIT                 NOT NULL DEFAULT 0,
    TenantId            UNIQUEIDENTIFIER    NULL,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_AbpBackgroundJobs PRIMARY KEY CLUSTERED (Id)
);
END
GO

-- ============================================================================
-- ABP Settings 表
-- ============================================================================

-- AbpSettings
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpSettings')
BEGIN
CREATE TABLE AbpSettings (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    Name                NVARCHAR(256)       NOT NULL,
    Value               NVARCHAR(MAX)       NULL,
    TenantId            UNIQUEIDENTIFIER    NULL,
    UserId              UNIQUEIDENTIFIER    NULL,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CreatorId           UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpSettings PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_AbpSettings_NameTenantUser UNIQUE (Name, TenantId, UserId)
);
END
GO

-- ============================================================================
-- ABP Features 表
-- ============================================================================

-- AbpFeatures
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpFeatures')
BEGIN
CREATE TABLE AbpFeatures (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    Name                NVARCHAR(128)       NOT NULL,
    Value               NVARCHAR(256)       NOT NULL,
    TenantId            UNIQUEIDENTIFIER    NULL,
    CreationTime        DATETIME2           NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_AbpFeatures PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_AbpFeatures_NameTenant UNIQUE (Name, TenantId)
);
END
GO

-- ============================================================================
-- ABP Language Texts 表
-- ============================================================================

-- AbpLanguageTexts
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AbpLanguageTexts')
BEGIN
CREATE TABLE AbpLanguageTexts (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    CultureName         NVARCHAR(10)        NOT NULL,
    ResourceName        NVARCHAR(256)       NOT NULL,
    Name                NVARCHAR(512)       NOT NULL,
    Value               NVARCHAR(MAX)       NULL,
    TenantId            UNIQUEIDENTIFIER    NULL,
    CONSTRAINT PK_AbpLanguageTexts PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_AbpLanguageTexts_CultureResourceName UNIQUE (CultureName, ResourceName, Name, TenantId)
);
END
GO

-- ============================================================================
-- AbpOrganizationUnits 表添加缺失字段
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'EntityVersion' AND object_id = OBJECT_ID('AbpOrganizationUnits'))
BEGIN
    ALTER TABLE AbpOrganizationUnits ADD EntityVersion INT NOT NULL DEFAULT 0;
END
GO

-- AbpOrganizationUnits 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_AbpOrganizationUnits_ParentId')
    CREATE NONCLUSTERED INDEX IDX_AbpOrganizationUnits_ParentId ON AbpOrganizationUnits (ParentId);
GO

-- ============================================================================
-- AbpUserRoles 表添加缺失字段
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'EntityVersion' AND object_id = OBJECT_ID('AbpUserRoles'))
BEGIN
    ALTER TABLE AbpUserRoles ADD EntityVersion INT NOT NULL DEFAULT 0;
END
GO

-- ============================================================================
-- 索引创建
-- ============================================================================

-- AbpUsers 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_AbpUsers_Email')
    CREATE NONCLUSTERED INDEX IDX_AbpUsers_Email ON AbpUsers (NormalizedEmail) WHERE IsDeleted = 0;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_AbpUsers_IsActive')
    CREATE NONCLUSTERED INDEX IDX_AbpUsers_IsActive ON AbpUsers (IsActive) WHERE IsDeleted = 0;
GO

-- AbpRoles 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_AbpRoles_IsStatic')
    CREATE NONCLUSTERED INDEX IDX_AbpRoles_IsStatic ON AbpRoles (IsStatic);
GO

PRINT 'ABP框架必需表和字段修复完成!';