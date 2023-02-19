using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data.Context;

namespace Shop.Infrastructure.Data;

public class NoSqlSyncDataBase : ISyncDataBase
{
    private readonly ReadDbContext _readDbContext;
    private readonly ILogger<NoSqlSyncDataBase> _logger;

    public NoSqlSyncDataBase(ReadDbContext readDbContext, ILogger<NoSqlSyncDataBase> logger)
    {
        _readDbContext = readDbContext;
        _logger = logger;
    }

    public async Task SaveAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel
    {
        var collection = _readDbContext.GetCollection<TQueryModel>();

        // ReplaceOptions:
        // Se o documento existir, será substituído.
        // Se o documento não existir, será criado um novo.
        var replaceOptions = new ReplaceOptions { IsUpsert = true };

        await GetRetryPolicy(_logger).ExecuteAsync(
            async () => await collection.ReplaceOneAsync(upsertFilter, queryModel, replaceOptions));
    }

    public async Task DeleteAsync<TQueryModel>(Expression<Func<TQueryModel, bool>> deleteFilter)
        where TQueryModel : IQueryModel
    {
        var collection = _readDbContext.GetCollection<TQueryModel>();
        await GetRetryPolicy(_logger).ExecuteAsync(async () => await collection.DeleteOneAsync(deleteFilter));
    }

    private static AsyncRetryPolicy GetRetryPolicy(ILogger<NoSqlSyncDataBase> logger)
    {
        return Policy
          .Handle<MongoException>()
          .WaitAndRetryAsync(2, (retryAttempt) =>
          {
              var jitterer = new Random();

              // Retry with jitter
              // A well-known retry strategy is exponential backoff, allowing retries to be made initially quickly,
              // but then at progressively longer intervals: for example, after 2, 4, 8, 15, then 30 seconds.
              // REF: https://github.com/App-vNext/Polly/wiki/Retry-with-jitter#simple-jitter
              var sleepDuration
                = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000));

              logger.LogWarning("----- MongoDB: Retry #{Count} with delay {Delay}", retryAttempt, sleepDuration);

              return sleepDuration;
          }, (ex, _) => logger.LogError(ex, "Ocorreu uma exceção não esperada ao salvar no MongoDB: {Message}", ex.Message));
    }
}