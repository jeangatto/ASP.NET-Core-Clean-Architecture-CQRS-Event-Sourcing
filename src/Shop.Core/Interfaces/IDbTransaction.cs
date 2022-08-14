using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shop.Core.Interfaces;

public interface IDbTransaction<TContext>
    where TContext : DbContext
{
    Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}