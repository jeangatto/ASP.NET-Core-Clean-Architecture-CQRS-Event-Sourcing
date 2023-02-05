using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shop.Core.Abstractions;

namespace Shop.Core.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task<TEntity> GetByIdAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}