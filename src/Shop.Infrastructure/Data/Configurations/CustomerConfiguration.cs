using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data.Configurations;

public class CustomerConfiguration : BaseConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(100);

        builder.Property(c => c.Gender)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(6)
            .HasConversion<string>();

        builder.Property(c => c.Email)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(254);

        builder.Property(c => c.DateOfBirth)
            .IsRequired()
            .HasColumnType("DATE");

        builder.HasIndex(c => c.Email).IsUnique(true);
    }
}