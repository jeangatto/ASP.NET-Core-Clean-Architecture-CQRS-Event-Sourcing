using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Configurations;

public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ConfigureBaseEntity();

        builder.Property(catalog => catalog.Name)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(50);

        builder.Property(catalog => catalog.Description)
            .IsRequired(false)
            .IsUnicode(false)
            .HasMaxLength(300);

        builder.Property(catalog => catalog.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(catalog => catalog.PictureUri)
            .IsRequired(false)
            .IsUnicode(false)
            .HasMaxLength(2048); // Maximum URL length

        builder.HasOne(catalog => catalog.CatalogBrand)
            .WithMany()
            .HasForeignKey(catalog => catalog.CatalogBrandId);

        builder.HasOne(catalog => catalog.CatalogType)
            .WithMany()
            .HasForeignKey(catalog => catalog.CatalogTypeId);
    }
}