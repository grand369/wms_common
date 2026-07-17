using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.RuleEngine.Application.Contracts.Permissions;

/// <summary>
/// Wms RuleEngine Permissions (PERM-RE).
/// </summary>
public class WmsRuleEnginePermissions
{
    public const string GroupName = "Wms.RuleEngine";

    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Update = GroupName + ".Update";
    public const string Execute = GroupName + ".Execute";
    public const string Import = GroupName + ".Import";
}

/// <summary>
/// RuleEngine Permission Definition Provider.
/// </summary>
public class WmsRuleEnginePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(WmsRuleEnginePermissions.GroupName, L("Rule Engine"));

        group.AddPermission(WmsRuleEnginePermissions.Read, L("Read Business Rules"));
        group.AddPermission(WmsRuleEnginePermissions.Create, L("Create Business Rules"));
        group.AddPermission(WmsRuleEnginePermissions.Update, L("Update Business Rules"));
        group.AddPermission(WmsRuleEnginePermissions.Execute, L("Execute Business Rules"));
        group.AddPermission(WmsRuleEnginePermissions.Import, L("Import Industry Packages"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
