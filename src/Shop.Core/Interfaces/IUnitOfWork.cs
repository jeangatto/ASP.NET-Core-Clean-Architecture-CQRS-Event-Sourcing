using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

public interface IUnitOfWork
{
    /// <summary>
    /// Salva todas as alterações feitas no contexto do banco de dados.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>O número de linhas afetadas no banco de dados.</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}