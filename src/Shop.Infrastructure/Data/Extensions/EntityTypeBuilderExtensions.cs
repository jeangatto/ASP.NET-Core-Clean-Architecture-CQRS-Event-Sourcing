using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Core.Abstractions;

namespace Shop.Infrastructure.Data.Extensions;

public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Configuração da entidade base.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="builder"></param>
    public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .IsRequired()
            .ValueGeneratedNever(); // O Id será gerado ao instanciar a classe

        builder.Ignore(entity => entity.DomainEvents);
    }

    /// <summary>
    /// Configuração da entidade base com auditoria, adicionando o filtro para obter somente
    /// as entidades que não estiverem deletadas (IsDeleted = false).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="builder"></param>
    public static void ConfigureBaseAuditEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseAuditEntity
    {
        builder.ConfigureBaseEntity();

        builder.Property(audit => audit.CreatedAt)
            .IsRequired();

        builder.Property(audit => audit.CreatedBy)
            .IsRequired(false);

        builder.Property(audit => audit.LastModified)
            .IsRequired(false);

        builder.Property(audit => audit.LastModifiedBy)
            .IsRequired(false);

        builder.Property(audit => audit.IsDeleted)
            .IsRequired();

        builder.Property(audit => audit.Version)
            .IsRequired();

        // Filtro universal para o SoftDelete.
        builder.HasQueryFilter(audit => !audit.IsDeleted);
    }
}