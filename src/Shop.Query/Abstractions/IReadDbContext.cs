using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Shop.Query.Abstractions;

public interface IReadDbContext
{
    string ConnectionString { get; }

    IMongoCollection<TQueryModel> GetCollection<TQueryModel>()
        where TQueryModel : IQueryModel;

    Task UpsertAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel;

    Task DeleteAsync<TQueryModel>(Expression<Func<TQueryModel, bool>> deleteFilter)
        where TQueryModel : IQueryModel;

    Task CreateCollectionsAsync();
}