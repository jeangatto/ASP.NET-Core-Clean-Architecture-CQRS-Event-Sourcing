using System;
using System.Threading.Tasks;

namespace Shop.Core.SharedKernel;

/// <summary>
/// Unidade de Trabalho.
/// Responsável por salvar as alterações na base de escrita e disparar os eventos.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task SaveChangesAsync();
}