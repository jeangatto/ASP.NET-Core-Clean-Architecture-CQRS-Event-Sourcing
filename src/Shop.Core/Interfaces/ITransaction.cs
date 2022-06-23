using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

public interface ITransaction
{
    Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default);
}