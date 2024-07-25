using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Mappings;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .ConfigureBaseEntity();

        builder
            .Property(customer => customer.FirstName)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(customer => customer.LastName)
            .IsRequired() // NOT NULL
            .HasMaxLength(100);

        builder
            .Property(customer => customer.Gender)
            .IsRequired() // NOT NULL
            .HasMaxLength(6)
            .HasConversion<string>();

        // Value Object Mapping (ValueObject)
        builder.OwnsOne(customer => customer.Email, ownedNav =>
        {
            ownedNav
                .Property(email => email.Address)
                .IsRequired() // NOT NULL
                .HasMaxLength(254)
                .HasColumnName(nameof(Customer.Email));

            // Unique Index
            ownedNav
                .HasIndex(email => email.Address)
                .IsUnique();
        });

        builder
            .Property(customer => customer.DateOfBirth)
            .IsRequired() // NOT NULL
            .HasColumnType("DATE");
    }
}