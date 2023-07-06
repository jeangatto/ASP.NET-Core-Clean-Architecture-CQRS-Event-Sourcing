using System;
using System.Threading.Tasks;

namespace Shop.Core.SharedKernel;

public interface IWriteOnlyRepository<TEntity> : IDisposable where TEntity : IEntity<Guid>
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<TEntity> GetByIdAsync(Guid id);
}