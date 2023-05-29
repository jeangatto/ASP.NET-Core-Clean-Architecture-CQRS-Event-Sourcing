using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.Shared;

namespace Shop.Infrastructure.Data.Mappings;

internal class EventStoreConfiguration : IEntityTypeConfiguration<EventStore>
{
    public void Configure(EntityTypeBuilder<EventStore> builder)
    {
        builder
            .HasKey(eventStore => eventStore.Id); // Primary Key

        builder
            .Property(eventStore => eventStore.Id)
            .IsRequired() // NOT NULL
            .ValueGeneratedNever(); // O Id será gerado ao instanciar a classe

        builder
            .Property(eventStore => eventStore.AggregateId)
            .IsRequired(); // NOT NULL

        builder
            .Property(eventStore => eventStore.MessageType)
            .IsRequired() // NOT NULL
            .IsUnicode(false) // VARCHAR
            .HasMaxLength(100);

        builder
            .Property(eventStore => eventStore.Data)
            .IsRequired() // NOT NULL
            .IsUnicode(false) // VARCHAR
            .HasColumnType("VARCHAR(MAX)")
            .HasComment("JSON serialized event");

        builder
            .Property(eventStore => eventStore.OccurredOn)
            .IsRequired() // NOT NULL
            .HasColumnName("CreatedAt");
    }
}