using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierAgg = Wms.Supplier.Domain.Aggregates.Supplier;

namespace Wms.Supplier.EntityFrameworkCore.Configurations;

/// <summary>
/// Supplier Entity Framework Core Configuration — defines table and column mappings.
/// </summary>
public class SupplierConfiguration : IEntityTypeConfiguration<SupplierAgg>
{
    public void Configure(EntityTypeBuilder<SupplierAgg> builder)
    {
        builder.ToTable("Suppliers");

        builder.Property(e => e.SupplierCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.SupplierCode)
            .IsUnique();

        builder.Property(e => e.SupplierName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ShortName)
            .HasMaxLength(100);

        builder.Property(e => e.SupplierType)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(e => e.ContactName)
            .HasMaxLength(100);

        builder.Property(e => e.ContactPhone)
            .HasMaxLength(50);

        builder.Property(e => e.ContactEmail)
            .HasMaxLength(100);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.City)
            .HasMaxLength(100);

        builder.Property(e => e.Province)
            .HasMaxLength(100);

        builder.Property(e => e.PostalCode)
            .HasMaxLength(20);

        builder.Property(e => e.TaxId)
            .HasMaxLength(50);

        builder.Property(e => e.BankName)
            .HasMaxLength(200);

        builder.Property(e => e.BankAccount)
            .HasMaxLength(50);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.Remark)
            .HasMaxLength(500);

        builder.Property(e => e.ErpSupplierCode)
            .HasMaxLength(50);
    }
}
