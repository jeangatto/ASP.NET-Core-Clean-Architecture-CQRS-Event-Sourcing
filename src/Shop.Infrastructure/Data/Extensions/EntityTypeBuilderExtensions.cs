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
        builder.Property(entity => entity.Id).IsRequired().ValueGeneratedNever();
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

        builder.Property(entity => entity.CreatedAt).IsRequired().ValueGeneratedNever();
        builder.Property(entity => entity.CreatedBy).IsRequired(false);
        builder.Property(entity => entity.LastModified).IsRequired(false);
        builder.Property(entity => entity.LastModifiedBy).IsRequired(false);
        builder.Property(entity => entity.IsDeleted).IsRequired();
        builder.Property(entity => entity.Version).IsRequired();

        // Filtro universal para o SoftDelete.
        builder.HasQueryFilter(entity => !entity.IsDeleted);
    }
}