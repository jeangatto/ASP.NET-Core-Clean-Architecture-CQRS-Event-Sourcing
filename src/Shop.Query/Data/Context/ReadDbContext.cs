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
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Context;

public class ReadDbContext : IReadDbContext
{
    private const string DatabaseName = "Shop";
    private static readonly Random Rnd = new();

    private readonly IMongoDatabase _database;
    private readonly ILogger<ReadDbContext> _logger;
    private readonly AsyncRetryPolicy _mongoRetryPolicy;

    public ReadDbContext(IOptions<ConnectionOptions> options, ILogger<ReadDbContext> logger)
    {
        ConnectionString = options.Value.NoSqlConnection;

        var client = new MongoClient(options.Value.NoSqlConnection);
        _database = client.GetDatabase(DatabaseName);
        _logger = logger;
        _mongoRetryPolicy = CreateRetryPolicy(logger);
    }

    public string ConnectionString { get; }

    public IMongoCollection<TQueryModel> GetCollection<TQueryModel>() where TQueryModel : IQueryModel
        => _database.GetCollection<TQueryModel>(typeof(TQueryModel).Name);

    public async Task UpsertAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel
    {
        var collection = GetCollection<TQueryModel>();

        // ReplaceOptions:
        // Se o documento existir, será substituído.
        // Se o documento não existir, será criado um novo.
        var replaceOptions = new ReplaceOptions { IsUpsert = true };

        await _mongoRetryPolicy
            .ExecuteAsync(async () => await collection.ReplaceOneAsync(upsertFilter, queryModel, replaceOptions));
    }

    public async Task DeleteAsync<TQueryModel>(Expression<Func<TQueryModel, bool>> deleteFilter)
        where TQueryModel : IQueryModel
    {
        var collection = GetCollection<TQueryModel>();
        await _mongoRetryPolicy
            .ExecuteAsync(async () => await collection.DeleteOneAsync(deleteFilter));
    }

    public async Task CreateCollectionsAsync()
    {
        // Obtendo as coleções existentes da base.
        using var asyncCursor = await _database.ListCollectionNamesAsync();
        var collections = await asyncCursor.ToListAsync();

        foreach (var collectionName in GetCollectionNamesFromAssembly())
        {
            if (!collections.Any(name => string.Equals(name, collectionName, StringComparison.Ordinal)))
            {
                _logger.LogInformation("----- MongoDB: criando a coleção {Name}", collectionName);
                await _database.CreateCollectionAsync(collectionName);
            }
            else
            {
                _logger.LogInformation("----- MongoDB: a coleção {Name} já existe", collectionName);
            }
        }

        await CreateIndexAsync();
    }

    private async Task CreateIndexAsync()
    {
        var indexDefinition = Builders<CustomerQueryModel>.IndexKeys.Ascending(model => model.Email);

        var indexModel = new CreateIndexModel<CustomerQueryModel>(indexDefinition, new CreateIndexOptions
        {
            // Restrição Exclusiva
            Unique = true,

            // Índices esparsos são como índices não esparsos, exceto que eles omitem referências a documentos que não incluem o campo indexado.
            // Para campos que estão presentes apenas em alguns documentos, índices esparsos podem fornecer uma economia significativa de espaço.
            Sparse = true
        });

        var collection = GetCollection<CustomerQueryModel>();
        await collection.Indexes.CreateOneAsync(indexModel);
    }

    private static IEnumerable<string> GetCollectionNamesFromAssembly()
        => Assembly
            .GetExecutingAssembly()
            .GetAllTypesOf<IQueryModel>()
            .Select(impl => impl.Name)
            .ToList();

    private static AsyncRetryPolicy CreateRetryPolicy(ILogger<ReadDbContext> logger)
    {
        return Policy
          .Handle<MongoException>()
          .WaitAndRetryAsync(2, (retryAttempt) =>
          {
              // Retry with jitter
              // A well-known retry strategy is exponential backoff, allowing retries to be made initially quickly,
              // but then at progressively longer intervals: for example, after 2, 4, 8, 15, then 30 seconds.
              // REF: https://github.com/App-vNext/Polly/wiki/Retry-with-jitter#simple-jitter
              var sleepDuration
                = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Rnd.Next(0, 1000));

              logger.LogWarning("----- MongoDB: Retry #{Count} with delay {Delay}", retryAttempt, sleepDuration);

              return sleepDuration;
          }, (ex, _) => logger.LogError(ex, "Ocorreu uma exceção não esperada ao salvar no MongoDB: {Message}", ex.Message));
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~ReadDbContext()
        => Dispose(false);

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;
    }

    #endregion
}