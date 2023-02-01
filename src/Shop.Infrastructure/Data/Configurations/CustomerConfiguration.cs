using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Entities.Customer;
using Shop.Infrastructure.Data.Extensions;

namespace Shop.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ConfigureBaseAuditEntity();

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
            // Convertendo o enumerador para string ao persistir no banco de dados.
            // Ao invés de salvar o valor (ex.: 0, 1, 3), salvará o nome do enumerador, facilitando a leitura no banco.
            .HasConversion<string>();

        builder.Property(c => c.Email)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(254);

        builder.Property(c => c.DateOfBirth)
            .IsRequired()
            .HasColumnType("DATE");

        // Índice único para o endereço de e-mail.
        builder.HasIndex(c => c.Email).IsUnique(true);
    }
}