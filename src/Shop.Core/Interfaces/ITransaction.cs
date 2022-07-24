using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shop.Core.Interfaces;

public interface ITransaction<TContext> where TContext : DbContext
{
    Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}