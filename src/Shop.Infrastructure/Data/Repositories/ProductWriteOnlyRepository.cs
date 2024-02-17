using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.ProductAggregate;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Data.Repositories.Common;

namespace Shop.Infrastructure.Data.Repositories;

internal class ProductWriteOnlyRepository(WriteDbContext context) : BaseWriteOnlyRepository<Product, Guid>(context), IProductWriteOnlyRepository
{
    public async Task<bool> ExistsByNameAsync(string name) =>
        await Context.Products
            .AsNoTracking()
            .AnyAsync(product => product.Name == name);

    public async Task<bool> ExistsByNameAsync(string name, Guid currentId) =>
        await Context.Products
            .AsNoTracking()
            .AnyAsync(product => product.Name == name && product.Id != currentId);
}