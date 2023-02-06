using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Salva todas as alterações feitas no contexto do banco de dados.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
}