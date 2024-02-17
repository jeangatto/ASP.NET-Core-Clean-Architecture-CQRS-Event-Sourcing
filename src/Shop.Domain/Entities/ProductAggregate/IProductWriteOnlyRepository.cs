using System;
using System.Threading.Tasks;
using Shop.Core.SharedKernel;

namespace Shop.Domain.Entities.ProductAggregate;

public interface IProductWriteOnlyRepository : IWriteOnlyRepository<Product, Guid>
{
    /// <summary>
    /// Checks if a product with the specified name already exists asynchronously.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>
    /// True if a product with the name exists, false otherwise.
    /// </returns>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// Checks if a product with the specified name and current ID already exists asynchronously.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <param name="currentId">The current ID of the product to exclude from the check.</param>
    /// <returns>
    /// True if a product with the name and current ID exists, false otherwise.
    /// </returns>
    Task<bool> ExistsByNameAsync(string name, Guid currentId);
}