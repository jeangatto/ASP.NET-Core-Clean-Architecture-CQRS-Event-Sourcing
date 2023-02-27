using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

/// <summary>
/// Unidade de Trabalho.
/// Responsável por salvar as alterações na base de escrita e disparar os eventos.
/// </summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync();
}