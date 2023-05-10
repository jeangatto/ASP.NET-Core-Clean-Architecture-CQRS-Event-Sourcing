using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Shop.Infrastructure.Data.Extensions;

internal static class ModelBuilderExtensions
{
    /// <summary>
    /// Remove a delação em cascata de chaves estrangeiras (FK).
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    internal static void RemoveCascadeDeleteConvention(this ModelBuilder modelBuilder)
    {
        var foreignKeys = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(entity => entity.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
            .ToList();

        foreach (var fk in foreignKeys)
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}