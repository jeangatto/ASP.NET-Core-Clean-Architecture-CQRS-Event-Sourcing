using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using Shop.Core.AppSettings;
using Shop.Core.Extensions;
using Shop.Query.Abstractions;

namespace Shop.Query.Data.Context;

public class ReadDbContext
{
    private const string DatabaseName = "Shop";

    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly ILogger<ReadDbContext> _logger;
    private readonly AsyncRetryPolicy _mongoRetryPolicy;

    public ReadDbContext(IOptions<ConnectionOptions> options, ILogger<ReadDbContext> logger)
    {
        ConnectionString = options.Value.NoSqlConnection;

        _client = new MongoClient(options.Value.NoSqlConnection);
        _database = _client.GetDatabase(DatabaseName);
        _logger = logger;
        _mongoRetryPolicy = CreateRetryPolicy(logger);
    }

    public string ConnectionString { get; }

    public IMongoCollection<T> GetCollection<T>() where T : IQueryModel => _database.GetCollection<T>(typeof(T).Name);

    public async Task UpsertAsync<T>(T queryModel, Expression<Func<T, bool>> upsertFilter) where T : IQueryModel
    {
        var collection = GetCollection<T>();

        // ReplaceOptions:
        // Se o documento existir, será substituído.
        // Se o documento não existir, será criado um novo.
        var replaceOptions = new ReplaceOptions { IsUpsert = true };

        await _mongoRetryPolicy
            .ExecuteAsync(async () => await collection.ReplaceOneAsync(upsertFilter, queryModel, replaceOptions));
    }

    public async Task DeleteAsync<T>(Expression<Func<T, bool>> deleteFilter) where T : IQueryModel
    {
        var collection = GetCollection<T>();
        await _mongoRetryPolicy
            .ExecuteAsync(async () => await collection.DeleteOneAsync(deleteFilter));
    }

    public async Task CreateCollectionsAsync()
    {
        // Obtendo as coleções existentes da base.
        using var asynCursor = await _database.ListCollectionNamesAsync();
        var collections = await asynCursor.ToListAsync();

        foreach (var collectionName in GetCollectionNamesFromAssembly())
        {
            if (!collections.Any(name => name == collectionName))
            {
                _logger.LogInformation("----- MongoDB: criando a coleção {Name}", collectionName);

                await _database.CreateCollectionAsync(collectionName);
            }
            else
            {
                _logger.LogInformation("----- MongoDB: a coleção {Name} já existe", collectionName);
            }
        }
    }

    private static IEnumerable<string> GetCollectionNamesFromAssembly()
    {
        return Assembly
            .GetExecutingAssembly()
            .GetAllTypesOfInterface<IQueryModel>()
            .Select(impl => impl.Name)
            .ToList();
    }

    private static AsyncRetryPolicy CreateRetryPolicy(ILogger<ReadDbContext> logger)
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