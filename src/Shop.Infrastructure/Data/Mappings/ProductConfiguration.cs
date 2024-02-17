using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities.ProductAggregate;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Mappings;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ConfigureBaseEntity();

        builder
            .Property(Product => Product.Name)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(Product => Product.Description)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(Product => Product.Price)
            .IsRequired(); // NOT NULL
    }
}