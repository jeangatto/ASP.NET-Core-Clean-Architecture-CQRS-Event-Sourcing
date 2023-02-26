using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data.Repositories;

public abstract class BaseWriteOnlyRepository<T> : IWriteOnlyRepository<T, Guid> where T : class, IEntity<Guid>
{
    protected readonly DbSet<T> DbSet;

    protected BaseWriteOnlyRepository(WriteDbContext context)
        => DbSet = context.Set<T>();

    public void Add(T entity)
        => DbSet.Add(entity);

    public void AddRange(IEnumerable<T> entities)
        => DbSet.AddRange(entities);

    public void Update(T entity)
        => DbSet.Update(entity);

    public void UpdateRange(IEnumerable<T> entities)
        => DbSet.UpdateRange(entities);

    public void Remove(T entity)
        => DbSet.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities)
        => DbSet.RemoveRange(entities);

    public async Task<T> GetByIdAsync(Guid id)
        => await DbSet.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);
}