using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Mappings;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ConfigureBaseEntity();

        builder
            .Property(customer => customer.FirstName)
            .IsRequired() // NOT NULL
            .IsUnicode(false) // VARCHAR
            .HasMaxLength(100);

        builder
            .Property(customer => customer.LastName)
            .IsRequired() // NOT NULL
            .IsUnicode(false) // VARCHAR
            .HasMaxLength(100);

        builder
            .Property(customer => customer.Gender)
            .IsRequired() // NOT NULL
            .IsUnicode(false) // VARCHAR
            .HasMaxLength(6)
            // Convertendo o enumerador para string ao persistir no banco de dados.
            // Ao invés de salvar o valor (ex.: 0, 1, 3), salvará o nome do enumerador, facilitando a leitura no banco.
            .HasConversion<string>();

        // Mapeamento de Objetos de Valor (ValueObject)
        builder.OwnsOne(customer => customer.Email, ownedNav =>
        {
            ownedNav
                .Property(email => email.Address)
                .IsRequired() // NOT NULL
                .IsUnicode(false) // VARCHAR
                .HasMaxLength(254)
                .HasColumnName(nameof(Customer.Email))
                .IsEncrypted(); // Encrypted Column, ref: https://github.com/Eastrall/EntityFrameworkCore.DataEncryption

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