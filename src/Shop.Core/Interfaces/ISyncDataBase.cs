using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

/// <summary>
/// Sincroniza a base de escrita com a de leitura.
/// </summary>
public interface ISyncDataBase
{
    /// <summary>
    /// Salva o modelo na base de leitura.
    /// </summary>
    /// <typeparam name="TQueryModel">O tipo do modelo.</typeparam>
    /// <param name="queryModel">O modelo.</param>
    /// <param name="upsertFilter">Filtro utilizado para efetuar o Upsert caso o documento já exista na base de leitura.</param>
    Task SaveAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter) where TQueryModel : IQueryModel;
}