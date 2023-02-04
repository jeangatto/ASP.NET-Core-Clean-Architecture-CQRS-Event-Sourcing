using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.Events;

namespace Shop.Infrastructure.Data.Configurations;

public class StoredEventConfiguration : IEntityTypeConfiguration<StoredEvent>
{
    public void Configure(EntityTypeBuilder<StoredEvent> builder)
    {
        builder.HasKey(storedEvent => storedEvent.Id);

        builder.Property(storedEvent => storedEvent.Id)
            .IsRequired()
            .ValueGeneratedNever(); // O Id será gerado ao instanciar a classe

        builder.Property(storedEvent => storedEvent.Type)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(100);

        builder.Property(storedEvent => storedEvent.Data)
            .IsRequired()
            .IsUnicode(false)
            .HasColumnType("VARCHAR(MAX)");

        builder.Property(storedEvent => storedEvent.OccurredOn)
            .IsRequired()
            .HasColumnName("CreatedAt");
    }
}