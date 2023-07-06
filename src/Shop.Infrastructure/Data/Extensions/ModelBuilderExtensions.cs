using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Shop.Infrastructure.Data.Extensions;

internal static class ModelBuilderExtensions
{
    /// <summary>
    /// Removes the cascade delete convention from the model builder.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    internal static void RemoveCascadeDeleteConvention(this ModelBuilder modelBuilder)
    {
        // Get all the foreign keys in the model that are not ownership and have cascade delete behavior
        var foreignKeys = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(entity => entity.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
            .ToList();

        // Change the delete behavior of each foreign key to restrict
        foreach (var fk in foreignKeys)
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}