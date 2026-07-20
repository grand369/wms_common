using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.BarcodeLabel.Application.Contracts.Permissions;

/// <summary>
/// Wms BarcodeLabel Permissions — defines all permission names for the BarcodeLabel module.
/// (PERM-BL)
/// </summary>
public class WmsBarcodeLabelPermissions
{
    public const string GroupName = "Wms.BarcodeLabel";

    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Generate = GroupName + ".Generate";
    public const string Print = GroupName + ".Print";
}

/// <summary>
/// BarcodeLabel Permission Definition Provider — registers module permissions with ABP.
/// </summary>
public class WmsBarcodeLabelPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(WmsBarcodeLabelPermissions.GroupName, L("条码标签"));

        group.AddPermission(WmsBarcodeLabelPermissions.Read, L("条码标签管理"));
        group.AddPermission(WmsBarcodeLabelPermissions.Create, L("创建标签"));
        group.AddPermission(WmsBarcodeLabelPermissions.Generate, L("生成条码"));
        group.AddPermission(WmsBarcodeLabelPermissions.Print, L("打印标签"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
