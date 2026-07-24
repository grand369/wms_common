using Volo.Abp.Domain.Entities.Auditing;

namespace Wms.Supplier.Domain.Aggregates;

/// <summary>
/// Supplier Aggregate Root — represents a supplier/vendor.
/// Inherits FullAuditedAggregateRoot for ABP audit fields and soft delete.
/// </summary>
public class Supplier : FullAuditedAggregateRoot<Guid>
{
    /// <summary>供应商编码（业务自然键，唯一）</summary>
    public string SupplierCode { get; private set; } = string.Empty;

    /// <summary>供应商名称</summary>
    public string SupplierName { get; private set; } = string.Empty;

    /// <summary>供应商简称</summary>
    public string? ShortName { get; private set; }

    /// <summary>供应商类型（1=普通供应商, 2=战略供应商, 3=委外加工商）</summary>
    public int SupplierType { get; private set; }

    /// <summary>联系人</summary>
    public string? ContactName { get; private set; }

    /// <summary>联系电话</summary>
    public string? ContactPhone { get; private set; }

    /// <summary>联系邮箱</summary>
    public string? ContactEmail { get; private set; }

    /// <summary>地址</summary>
    public string? Address { get; private set; }

    /// <summary>城市</summary>
    public string? City { get; private set; }

    /// <summary>省份</summary>
    public string? Province { get; private set; }

    /// <summary>邮政编码</summary>
    public string? PostalCode { get; private set; }

    /// <summary>税号</summary>
    public string? TaxId { get; private set; }

    /// <summary>开户行</summary>
    public string? BankName { get; private set; }

    /// <summary>银行账号</summary>
    public string? BankAccount { get; private set; }

    /// <summary>是否启用</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>备注</summary>
    public string? Remark { get; private set; }

    /// <summary>ERP系统供应商编码（对接第三方系统时使用）</summary>
    public string? ErpSupplierCode { get; private set; }

    private Supplier() { }

    public Supplier(Guid id, string supplierCode, string supplierName)
        : base(id)
    {
        SupplierCode = supplierCode;
        SupplierName = supplierName;
    }

    public void Update(string supplierName, string? shortName, int supplierType,
        string? contactName, string? contactPhone, string? contactEmail,
        string? address, string? city, string? province, string? postalCode,
        string? taxId, string? bankName, string? bankAccount, bool isActive,
        string? remark, string? erpSupplierCode)
    {
        SupplierName = supplierName;
        ShortName = shortName;
        SupplierType = supplierType;
        ContactName = contactName;
        ContactPhone = contactPhone;
        ContactEmail = contactEmail;
        Address = address;
        City = city;
        Province = province;
        PostalCode = postalCode;
        TaxId = taxId;
        BankName = bankName;
        BankAccount = bankAccount;
        IsActive = isActive;
        Remark = remark;
        ErpSupplierCode = erpSupplierCode;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
