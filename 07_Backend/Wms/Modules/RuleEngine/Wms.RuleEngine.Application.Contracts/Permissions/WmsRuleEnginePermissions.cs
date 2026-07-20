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
        var group = context.AddGroup(WmsRuleEnginePermissions.GroupName, L("规则引擎"));

        group.AddPermission(WmsRuleEnginePermissions.Read, L("规则引擎管理"));
        group.AddPermission(WmsRuleEnginePermissions.Create, L("创建规则"));
        group.AddPermission(WmsRuleEnginePermissions.Update, L("更新规则"));
        group.AddPermission(WmsRuleEnginePermissions.Execute, L("执行规则"));
        group.AddPermission(WmsRuleEnginePermissions.Import, L("导入行业包"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
