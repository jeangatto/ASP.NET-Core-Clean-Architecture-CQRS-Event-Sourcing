using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.ValueObjects;
using Shop.Infrastructure.Data.Context;
using Shop.Infrastructure.Data.Repositories.Common;

namespace Shop.Infrastructure.Data.Repositories;

internal class CustomerWriteOnlyRepository(WriteDbContext context)
    : BaseWriteOnlyRepository<Customer, Guid>(context), ICustomerWriteOnlyRepository
{
    private static readonly Func<WriteDbContext, string, Task<bool>> ExistsByEmailCompiledAsync =
        EF.CompileAsyncQuery((WriteDbContext context, string email) =>
            context
                .Customers
                .AsNoTracking()
                .Any(customer => customer.Email.Address == email));

    private static readonly Func<WriteDbContext, string, Guid, Task<bool>> ExistsByEmailAndIdCompiledAsync =
        EF.CompileAsyncQuery((WriteDbContext context, string email, Guid currentId) =>
            context
                .Customers
                .AsNoTracking()
                .Any(customer => customer.Email.Address == email && customer.Id != currentId));

    public async Task<bool> ExistsByEmailAsync(Email email) =>
        await ExistsByEmailCompiledAsync(Context, email.Address);

    public async Task<bool> ExistsByEmailAsync(Email email, Guid currentId) =>
        await ExistsByEmailAndIdCompiledAsync(Context, email.Address, currentId);
}