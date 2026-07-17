using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Wms.Web.Host.Auth;

/// <summary>
/// Concrete implementation of PermissionManagementProvider for User-level permissions.
/// In ABP 8.3.0, PermissionManagementProvider is abstract; the framework no longer
/// ships RolePermissionManagementProvider / UserPermissionManagementProvider as
/// built-in classes. The application must provide concrete subclasses that override
/// the abstract Name property.
/// 
/// ProviderName "U" is the ABP convention for user-level permission management.
/// All other methods (SetAsync, CheckAsync, GrantAsync, RevokeAsync) are inherited
/// from the base class and use IPermissionGrantRepository for database operations.
/// </summary>
public class UserPermissionManagementProvider : PermissionManagementProvider
{
    public override string Name => "U";

    public UserPermissionManagementProvider(
        IPermissionGrantRepository permissionGrantRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
        : base(permissionGrantRepository, guidGenerator, currentTenant)
    {
    }
}
