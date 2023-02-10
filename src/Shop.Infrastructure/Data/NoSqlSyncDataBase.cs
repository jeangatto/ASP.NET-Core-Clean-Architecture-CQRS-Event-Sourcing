using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

public class NoSqlSyncDataBase : ISyncDataBase
{
    private static readonly AsyncRetryPolicy MongoRetryPolicy = Policy
        .Handle<MongoException>()
        .WaitAndRetryAsync(2, retryAttempt =>
        {
            // Retry with jitter
            // A well-known retry strategy is exponential backoff, allowing retries to be made initially quickly,
            // but then at progressively longer intervals: for example, after 2, 4, 8, 15, then 30 seconds.
            // REF: https://github.com/App-vNext/Polly/wiki/Retry-with-jitter#simple-jitter
            var jitterer = new Random();

            // exponential back-off: 2, 4, 8 etc
            var t1 = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));

            // plus some jitter: up to 1 second
            var t2 = TimeSpan.FromMilliseconds(jitterer.Next(0, 1000));
            return t1 + t2;
        });

    private readonly ReadDbContext _readDbContext;

    public NoSqlSyncDataBase(ReadDbContext readDbContext)
        => _readDbContext = readDbContext;

    public async Task SaveAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel
    {
        var collection = _readDbContext.GetCollection<TQueryModel>();

        await MongoRetryPolicy.ExecuteAsync(async () =>
        {
            // Se o documento existir, será substituído.
            // Se o documento não existir, será criado um novo.
            await collection.ReplaceOneAsync(upsertFilter, queryModel, new ReplaceOptions { IsUpsert = true });
        });
    }
}