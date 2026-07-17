USE WmsDb;
GO

-- ============================================================================
-- AbpUsers 表 NULL 值修复
-- ============================================================================

-- 将 NULL 值更新为默认值
UPDATE AbpUsers
SET 
    Name = COALESCE(Name, ''),
    Surname = COALESCE(Surname, ''),
    Email = COALESCE(Email, ''),
    NormalizedEmail = COALESCE(NormalizedEmail, ''),
    PasswordHash = COALESCE(PasswordHash, ''),
    SecurityStamp = COALESCE(SecurityStamp, ''),
    ConcurrencyStamp = COALESCE(ConcurrencyStamp, ''),
    PhoneNumber = COALESCE(PhoneNumber, ''),
    LastPasswordChangeTime = COALESCE(LastPasswordChangeTime, CreationTime),
    ShouldChangePasswordOnNextLogin = COALESCE(ShouldChangePasswordOnNextLogin, 0)
WHERE 
    Name IS NULL 
    OR Surname IS NULL 
    OR Email IS NULL 
    OR NormalizedEmail IS NULL 
    OR PasswordHash IS NULL 
    OR SecurityStamp IS NULL 
    OR ConcurrencyStamp IS NULL 
    OR PhoneNumber IS NULL
    OR LastPasswordChangeTime IS NULL
    OR ShouldChangePasswordOnNextLogin IS NULL;

PRINT 'AbpUsers 表 NULL 值修复完成!'
GO

-- ============================================================================
-- AbpRoles 表 NULL 值修复
-- ============================================================================

UPDATE AbpRoles
SET 
    ConcurrencyStamp = COALESCE(ConcurrencyStamp, '')
WHERE 
    ConcurrencyStamp IS NULL;

PRINT 'AbpRoles 表 NULL 值修复完成!'
GO

-- ============================================================================
-- AbpUserRoles 表 NULL 值修复
-- ============================================================================

UPDATE AbpUserRoles
SET 
    TenantId = COALESCE(TenantId, CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER))
WHERE 
    TenantId IS NULL;

PRINT 'AbpUserRoles 表 NULL 值修复完成!'
GO

-- ============================================================================
-- AbpOrganizationUnits 表 NULL 值修复
-- ============================================================================

UPDATE AbpOrganizationUnits
SET 
    ParentId = COALESCE(ParentId, CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER)),
    TenantId = COALESCE(TenantId, CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER))
WHERE 
    ParentId IS NULL 
    OR TenantId IS NULL;

PRINT 'AbpOrganizationUnits 表 NULL 值修复完成!'
GO

-- ============================================================================
-- 验证修复结果
-- ============================================================================

SELECT 
    'AbpUsers' AS TableName,
    COUNT(*) AS TotalRows,
    SUM(CASE WHEN Name IS NULL THEN 1 ELSE 0 END) AS NullNameCount,
    SUM(CASE WHEN Surname IS NULL THEN 1 ELSE 0 END) AS NullSurnameCount,
    SUM(CASE WHEN Email IS NULL THEN 1 ELSE 0 END) AS NullEmailCount,
    SUM(CASE WHEN PasswordHash IS NULL THEN 1 ELSE 0 END) AS NullPasswordHashCount
FROM AbpUsers
UNION ALL
SELECT 
    'AbpRoles' AS TableName,
    COUNT(*),
    0,
    0,
    0,
    SUM(CASE WHEN ConcurrencyStamp IS NULL THEN 1 ELSE 0 END)
FROM AbpRoles;

PRINT '验证完成!';