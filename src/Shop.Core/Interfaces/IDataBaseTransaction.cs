using System;
using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

public interface IDataBaseTransaction
{
    Task ExecuteAsync(Func<Task> action);
}
