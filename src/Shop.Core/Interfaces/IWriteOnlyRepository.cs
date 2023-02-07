using System.Collections.Generic;
using Shop.Core.Abstractions;

namespace Shop.Core.Interfaces;

public interface IWriteOnlyRepository<TEntity> where TEntity : BaseEntity
{
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}